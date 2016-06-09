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
using DevExpress.Utils;
namespace DevExpress.Charts.Native {
	public class GridSpacingCalculator {
		public static double Calculate(double axisRangeDelta, double screenDelta, double gridSpacingFactor) {
			ChartDebug.Assert(axisRangeDelta > 0, "axisRangeDelta must be > 0");
			ChartDebug.Assert(gridSpacingFactor > 0, "gridSpacingFactor must be > 0");
			if (axisRangeDelta <= 0)
				return 1;
			if (screenDelta <= 0)
				return axisRangeDelta;
			return MultiplierChooser.ChooseMultiplier(gridSpacingFactor * axisRangeDelta / screenDelta);
		}
	}
	public class InterlacedData {
		double near;
		double far;
		public double Near { get { return near; } }
		public double Far { get { return far; } }
		public InterlacedData(double near, double far) {
			this.near = near;
			this.far = far;
		}
	}
	public class GridAndTextDataEx {
		readonly IEnumerable<ISeries> seriesList;
		readonly IAxisData axis;
		readonly double axisLength;
		readonly double delta;
		readonly AxisGridDataEx gridData;
		readonly AxisTextDataEx textData;
		IAxisGridMapping mapping;
		bool IsLogarithmic {
			get {
				ILogarithmic logarithmic = axis as ILogarithmic;
				return logarithmic != null && logarithmic.Enabled && axis.AxisScaleTypeMap != null && axis.AxisScaleTypeMap.ScaleType == ActualScaleType.Numerical;
			}
		}
		IScaleOptionsBase ActualScaleOptions {
			get {
				IScaleOptionsBase scaleOptions = axis.QualitativeScaleOptions;
				if (axis.AxisScaleTypeMap != null) {
					switch (axis.AxisScaleTypeMap.ScaleType) {
						case ActualScaleType.DateTime:
							scaleOptions = axis.DateTimeScaleOptions;
							break;
						case ActualScaleType.Numerical:
							scaleOptions = axis.NumericScaleOptions;
							break;
						case ActualScaleType.Qualitative:
							scaleOptions = axis.QualitativeScaleOptions;
							break;
						default:
							ChartDebug.Fail("Unknown scale type");
							break;
					}
				}
				return scaleOptions;
			}
		}
		public double AxisLength { get { return axisLength; } }
		public AxisGridDataEx GridData { get { return gridData; } }
		public AxisTextDataEx TextData { get { return textData; } }
		public IAxisGridMapping Mapping {
			get {
				if (mapping == null)
					mapping = axis.GridMapping;
				return mapping;
			}
		}
		public GridAndTextDataEx(IEnumerable<ISeries> seriesList, IAxisData axis, bool scrollingEnabled, IMinMaxValues visualRange, IMinMaxValues wholeRange, double axisLength, bool staggered) {
			this.seriesList = seriesList;
			this.axis = axis;
			this.axisLength = axisLength;
			delta = CalculateDelta(visualRange);
			double gridSpacing = CalculateGridSpacing();
			gridData = new AxisGridDataEx(axis, Mapping, axis.IsRadarAxis, axis.MinorCount, scrollingEnabled, visualRange, gridSpacing);
			textData = new AxisTextDataEx(axis, Mapping, axis.IsRadarAxis, scrollingEnabled);
			CustomAxisElementsHelperNew.FillCustomTextData(axis, axis.GridMapping, visualRange, gridData, textData);
			bool hasCustomData = textData.Count != 0;
			if (axis.CanShowCustomWithAutoLabels || !hasCustomData)
				textData.Add(gridData);
			if (hasCustomData)
				textData.Sort();
			textData.Calculate(staggered);
		}
		internal double CalculateGridSpacing() {
			double gridSpacing = 1;
			if (axis.AxisScaleTypeMap != null) {
				if (IsLogarithmic)
					return CalculateLogarithmicGridSpacing();
				if (axis.AxisScaleTypeMap.ScaleType == ActualScaleType.Numerical || (axis.AxisScaleTypeMap.ScaleType == ActualScaleType.DateTime && axis.IsRadarAxis)) {
					gridSpacing = CalculateNumericGridSpacing();
					if (ActualScaleOptions.GridSpacingAuto)
						ActualScaleOptions.GridSpacing = gridSpacing;
				}
				if (axis.AxisScaleTypeMap.ScaleType == ActualScaleType.DateTime) {
					axis.DateTimeScaleOptions.Calculator.UpdateAutomaticGrid(axisLength, seriesList);
					gridSpacing = axis.DateTimeScaleOptions.GridSpacingAuto ? axis.DateTimeScaleOptions.GridSpacing : CalculateManualGridSpacing();
				}
			}
			return (gridSpacing == 0.0) ? 1.0 : gridSpacing;
		}
		double CalculateManualGridSpacing() {
			double gridSpacing = ActualScaleOptions.GridSpacing;
			double screenDelta = axisLength;
			if (screenDelta < 1)
				screenDelta = 1;
			double minGridSpacing = Mapping.InternalToAligned(delta) / screenDelta;
			if (gridSpacing < minGridSpacing)
				gridSpacing = minGridSpacing;
			if (IsLogarithmic)
				gridSpacing = (int)(gridSpacing + 0.5);
			return gridSpacing;
		}
		double CalculateLogarithmicGridSpacing() {
			double gridSpacing;
			LogarithmicTransformation transformation = Mapping.Transformation as LogarithmicTransformation;
			if (ActualScaleOptions.GridSpacingAuto)
				gridSpacing = GridSpacingCalculator.Calculate(delta, axisLength, axis.GridSpacingFactor);
			else
				gridSpacing = CalculateManualGridSpacing();
			if (delta <= 1)
				return gridSpacing == 0 ? 1 : gridSpacing;
			return gridSpacing < 1 ? 1 : Math.Floor(gridSpacing);
		}
		double CalculateNumericGridSpacing() {
			double gridSpacing = ActualScaleOptions.GridSpacingAuto ? Mapping.InternalToAligned(GridSpacingCalculator.Calculate(delta, axisLength, axis.GridSpacingFactor)) :
				CalculateManualGridSpacing();
			if (axis.AxisScaleTypeMap.ScaleType == ActualScaleType.Numerical && axis.NumericScaleOptions.ScaleMode != ScaleModeNative.Continuous)
				gridSpacing = Math.Max(1, gridSpacing);
			return gridSpacing;
		}
		double CalculateDelta(IMinMaxValues visualRange) {
			if (IsLogarithmic) {
				Transformation transformation = axis.AxisScaleTypeMap.Transformation;
				double delta = transformation.TransformForward(visualRange.Max) - transformation.TransformForward(visualRange.Min);
				return delta > 0 ? delta : 1;
			}
			return visualRange.Max - visualRange.Min;
		}
	}
	public class AxisTextDataEx {
		static object ConstructAxisLabelText(double value, IAxisData axisData, IAxisGridMapping mapping) {
			object nativeValue = mapping.InternalToNative(value);
			if (axisData.LabelFormatter != null)
				return axisData.LabelFormatter.GetAxisLabelText(nativeValue);
			string pattern = axisData.Label.TextPattern;
			PatternParser patternParser = new PatternParser(pattern, (IPatternHolder)axisData.Label);
			patternParser.SetContext(nativeValue);
			return patternParser.GetText();
		}
		IAxisData axisData;
		IAxisGridMapping mapping;
		readonly bool isRadarAxis;
		readonly bool scrollingEnabled;
		readonly List<AxisTextItem> items = new List<AxisTextItem>();
		List<AxisTextItem> primaryItems;
		List<AxisTextItem> staggeredItems;
		public AxisTextItem this[int index] { get { return items[index]; } }
		public int Count { get { return items.Count; } }
		public IList<AxisTextItem> PrimaryItems { get { return primaryItems; } }
		public IList<AxisTextItem> StaggeredItems { get { return staggeredItems; } }
		public AxisTextDataEx(IAxisData axisData, IAxisGridMapping mapping, bool isRadarAxis, bool scrollingEnabled) {
			this.axisData = axisData;
			this.mapping = mapping;
			this.isRadarAxis = isRadarAxis;
			this.scrollingEnabled = scrollingEnabled;
		}
		void CalculatePrimaryItems() {
			int count = Count / 2 + Count % 2;
			primaryItems = new List<AxisTextItem>(count);
			for (int i = 0, j = 0; i < count; i++, j += 2)
				primaryItems.Add(this[j]);
		}
		void CalculateStaggeredItems() {
			int count = Count / 2;
			staggeredItems = new List<AxisTextItem>(count);
			for (int i = 0, j = 1; i < count; i++, j += 2)
				staggeredItems.Add(this[j]);
		}
		object ConstructText(double value) {
			return ConstructAxisLabelText(value, axisData, mapping);
		}
		void AddAutomaticValue(AxisGridDataEx gridData, double value) {
			Add(gridData, null, value, ConstructText(value), false);
		}
		public void Add(AxisGridDataEx gridData, ICustomAxisLabel customAxisLabel, double value, object content, bool isCustom) {
			if (content != null) {
				if (gridData.ValueWithinRange(value))
					items.Add(new AxisTextItem(customAxisLabel, gridData.GridIndex(value), value, content, true, isCustom));
				else if (scrollingEnabled)
					items.Add(new AxisTextItem(customAxisLabel, gridData.GridIndex(value), value, content, false, isCustom));
			}
		}
		public void Add(AxisGridDataEx gridData) {
			if (isRadarAxis)
				foreach (AxisGridItem item in gridData.Items) {
					if (item.Visible && item.IsAutogenerated)
						items.Add(new AxisTextItem(null, gridData.GridIndex(item.Value), item.Value, ConstructText(item.Value), true, false));
				}
			else if (scrollingEnabled)
				foreach (AxisGridItem item in gridData.Items) {
					if (item.IsAutogenerated)
						AddAutomaticValue(gridData, item.Value);
				}
			else
				foreach (AxisGridItem item in gridData.Items) {
					if (item.Visible && item.IsAutogenerated)
						AddAutomaticValue(gridData, item.Value);
				}
		}
		public void Sort() {
			items.Sort();
		}
		public void Calculate(bool staggered) {
			if (staggered) {
				CalculatePrimaryItems();
				CalculateStaggeredItems();
			}
			else {
				primaryItems = items;
				staggeredItems = new List<AxisTextItem>();
			}
		}
	}
	public class AxisGridDataEx {
		abstract class GridValueProvider {
			double gridSpacing;
			IMinMaxValues range;
			IAxisGridMapping map;
			internal double GridSpacing { get { return gridSpacing; } }
			internal IMinMaxValues Range { get { return range; } }
			internal IAxisGridMapping Map { get { return map; } }
			public GridValueProvider(double gridSpacing, IMinMaxValues range, IAxisGridMapping map) {
				this.gridSpacing = gridSpacing;
				this.range = range;
				this.map = map;
			}
			internal abstract double[] GetGridValues(int index);
		}
		class IdentityGridValueProvider : GridValueProvider {
			internal IdentityGridValueProvider(double gridSpacing, IMinMaxValues range, IAxisGridMapping map)
				: base(gridSpacing, range, map) {
			}
			internal override double[] GetGridValues(int index) {
				double resultIndex = Math.Floor(Map.InternalToAligned(Range.Min) / GridSpacing) + index;
				return new double[] { GridSpacing * resultIndex };
			}
		}
		class LogarithmicGridValueProvider : GridValueProvider {
			readonly LogarithmicTransformation transformation;
			double delta;
			MinMaxValues range;
			double baseValue;
			double factor = 1;
			internal LogarithmicGridValueProvider(double gridSpacing, IMinMaxValues range, IAxisGridMapping map)
				: base(gridSpacing, range, map) {
				this.transformation = map.Transformation as LogarithmicTransformation;
				delta = transformation.TransformForward(range.Max) - transformation.TransformForward(range.Min);
				delta = delta > 0 ? delta : 1;
				baseValue = GetBaseValue();
				this.range = new MinMaxValues(range);
				factor = Math.Pow(2, Math.Floor(Math.Abs(Math.Log(delta, 2))));
			}
			double GetCorrectedLogarithmicValue(double value) {
				double logStart = transformation.TransformForward(value);
				if (logStart == 0)
					return logStart;
				double logStartBackward = transformation.TransformBackward(logStart);
				double diff = value - logStartBackward;
				if (diff < 0)
					return Math.Floor(logStart);
				else if (diff > 0)
					return Math.Ceiling(logStart);
				return logStart;
			}
			double GetBaseValue() {
				LogarithmicTransformation logarithmicTransformation = transformation as LogarithmicTransformation;
				double min = Math.Log(Math.Abs(Range.Min), logarithmicTransformation.LogarithmicBase);
				double majorBase = Math.Floor(Map.InternalToAligned(min));
				return Math.Pow(logarithmicTransformation.LogarithmicBase, majorBase);
			}
			double GetStartValue() {
				if (delta < 1) {
					if (Math.Abs(Range.Min) < 1.0)
						return Math.Floor(Range.Min + 10) / 10;
					return Math.Floor(Range.Min);
				}
				LogarithmicTransformation logarithmicTransformation = transformation as LogarithmicTransformation;
				double min = Math.Log(Math.Abs(Range.Min), logarithmicTransformation.LogarithmicBase);
				double majorBase = Math.Floor(Map.InternalToAligned(min));
				double startValue = Math.Pow(logarithmicTransformation.LogarithmicBase, majorBase);
				return startValue;
			}
			double[] GridValueForSmallRange(int gridIndex) {
				double minLog = Math.Floor(transformation.TransformForward(range.Min)) + gridIndex;
				double maxLog = minLog + 1;
				double min = transformation.TransformBackward(minLog);
				double max = transformation.TransformBackward(maxLog);
				double step;
				if (min == 0)
					step = Math.Pow(transformation.LogarithmicBase, maxLog - 2) / factor;
				else
					step = min / factor;
				long index = (long)Math.Ceiling(((range.Min - min) / step)) - 1;
				List<double> values = new List<double>();
				do {
					double value = min + index * step;
					if ((value > max && max <= range.Max) || (value > range.Max && max >= range.Max))
						break;
					values.Add(value);
					index++;
				} while (true);
				if (max > range.Max)
					values.Add(values[values.Count - 1] + step);
				return values.ToArray();
			}
			internal override double[] GetGridValues(int index) {
				double start = GetStartValue();
				if (delta < 1 && Range.Min != 0)
					return GridValueForSmallRange(index);
				if (index == 0)
					return new double[] { start };
				double logStart = GetCorrectedLogarithmicValue(start);
				double logValue = logStart + index * GridSpacing;
				return new double[] { transformation.TransformBackward(logValue) };
			}
		}
		static bool IsStripVisible(IAxisGridMapping map, IStrip strip) {
			if (!strip.Visible)
				return false;
			IStripLimit minLimit = strip.MinLimit;
			IStripLimit maxLimit = strip.MaxLimit;
			object minAxisValue = minLimit.IsEnabled ? minLimit.GetAxisValue() : null;
			object maxAxisValue = maxLimit.IsEnabled ? maxLimit.GetAxisValue() : null;
			if ((minAxisValue != null && !map.IsCompatible(minAxisValue)) || (maxAxisValue != null && !map.IsCompatible(maxAxisValue)))
				return false;
			return minAxisValue == null || maxAxisValue == null || map.NativeToInternal(maxAxisValue) > map.NativeToInternal(minAxisValue);
		}
		static double NormalizeStripValue(IMinMaxValues limits, double val) {
			if (Double.IsPositiveInfinity(val))
				return limits.Max;
			if (Double.IsNegativeInfinity(val))
				return limits.Min;
			if (val < limits.Min)
				return limits.Min;
			if (val > limits.Max)
				return limits.Max;
			return val;
		}
		static double CalcStripGridValue(IStrip strip, IMinMaxValues limits) {
			double minValue = NormalizeStripValue(limits, strip.MinLimit.Value);
			double maxValue = NormalizeStripValue(limits, strip.MaxLimit.Value);
			return minValue < maxValue ? (minValue + maxValue) / 2.0 : Double.NaN;
		}
		static bool IsStripVisibleInGrid(IMinMaxValues limits, IAxisGridMapping map, IStrip strip) {
			return IsStripVisible(map, strip) && !String.IsNullOrEmpty(strip.AxisLabelText) &&
				!Double.IsNaN(CalcStripGridValue(strip, limits));
		}
		static bool IsAxisValueVisible(IAxisData axis, object axisValue) {
			if (!axis.AxisScaleTypeMap.IsCompatible(axisValue))
				return false;
			return axis.AxisScaleTypeMap.ScaleType != ActualScaleType.DateTime || axis.DateTimeScaleOptions.WorkdaysOptions == null || !axis.DateTimeScaleOptions.WorkdaysOptions.WorkdaysOnly ||
				!DateTimeUtils.IsHoliday(axis.DateTimeScaleOptions.WorkdaysOptions, (DateTime)axisValue, true, true);
		}
		static GridValueProvider CreateGridValueProvider(double gridSpacing, IAxisGridMapping mapping, IMinMaxValues visualRange) {
			if (mapping.Transformation.IsIdentity) {
				return new IdentityGridValueProvider(gridSpacing, visualRange, mapping);
			}
			else {
				return new LogarithmicGridValueProvider(gridSpacing, visualRange, mapping);
			}
		}
		static double CalculateDelta(IAxisGridMapping mapping, IMinMaxValues visualRange) {
			if (!mapping.Transformation.IsIdentity) {
				Transformation transformation = mapping.Transformation;
				double delta = transformation.TransformForward(visualRange.Max) - transformation.TransformForward(visualRange.Min);
				return delta > 0 ? delta : 1;
			}
			return visualRange.Max - visualRange.Min;
		}
		static bool IsValidValue(double value) {
			return !Double.IsPositiveInfinity(value) && !Double.IsNegativeInfinity(value) && !Double.IsNaN(value);
		}
		readonly IAxisElementContainer axis;
		readonly bool isRadarAxis;
		readonly bool scrollingEnabled;
		readonly IAxisGridMapping mapping;
		readonly IMinMaxValues visualRange;
		readonly double gridSpacing;
		readonly double epsilon;
		readonly int minorStep;
		readonly int minorCount;
		readonly AxisGridItemList items = new AxisGridItemList();
		readonly List<double> minorValues = new List<double>();
		readonly List<InterlacedData> interlacedData = new List<InterlacedData>();
		GridValueProvider gridValueProvider;
		bool interlacedInverse = false;
		public AxisGridItemList Items { get { return items; } }
		public List<double> MinorValues { get { return minorValues; } }
		public List<InterlacedData> InterlacedData { get { return interlacedData; } }
		public bool InterlacedInverse { get { return interlacedInverse; } }
		public AxisGridDataEx(IAxisElementContainer axis, IAxisGridMapping mapping, bool isRadarAxis, int minorCount, bool scrollingEnabled, IMinMaxValues visualRange, double gridSpacing) {
			gridValueProvider = CreateGridValueProvider(gridSpacing, mapping, visualRange);
			this.axis = axis;
			this.mapping = mapping;
			this.isRadarAxis = isRadarAxis;
			this.scrollingEnabled = scrollingEnabled;
			this.visualRange = visualRange;
			this.gridSpacing = gridSpacing;
			epsilon = (visualRange.Max - visualRange.Min) * 1e-10;
			if (epsilon == 0.0)
				epsilon = Double.Epsilon;
			this.minorStep = minorCount + 1;
			this.minorCount = minorCount - 1;
			bool canShowCustomWithAutoLabels = ((IAxisData)axis).CanShowCustomWithAutoLabels;
			if (canShowCustomWithAutoLabels || !CreateStripsAndCustomLabels())
				FillAutomaticValues();
			CreateMinorValues();
			CreateInterlacedData();
			if (canShowCustomWithAutoLabels)
				CreateStripsAndCustomLabels();
		}
		void ExtendRadarAxisRange() {
			double minValue = items.First.Value;
			double maxValue = items.Last.Value;
			if (minValue < items.MinVisibleValue)
				items[0] = new AxisGridItem() { Value = minValue, IsAutogenerated = true, Visible = true };
			if (maxValue > items.MaxVisibleValue)
				items[items.Count - 1] = new AxisGridItem() { Value = maxValue, IsAutogenerated = true, Visible = true };
		}
		void FillAutomaticValues() {
			CreateMajorValues();
			if (isRadarAxis && items.VisibleItemsCount > 0 && items.InvisibleItemsCount > 0)
				ExtendRadarAxisRange();
		}
		bool CreateStripsAndCustomLabels() {
			if (axis.Strips != null) {
				foreach (IStrip strip in axis.Strips) {
					if (IsStripVisibleInGrid(visualRange, mapping, strip)) {
						AddCustomValue(strip.MinLimit.Value);
						AddCustomValue(strip.MaxLimit.Value);
					}
				}
			}
			if (axis.CustomLabels != null) {
				foreach (ICustomAxisLabel label in axis.CustomLabels)
					if (label.Visible && IsAxisValueVisible((IAxisData)axis, label.GetAxisValue()))
						AddCustomValue(label.GetValue());
			}
			return (items.Count > 0);
		}
		double[] GetGridValue(GridValueProvider provider, int index) {
			double[] values = gridValueProvider.GetGridValues(index);
			for (int i = 0; i < values.Length; i++)
				values[i] = mapping.AlignedToInternal(values[i]);
			return values;
		}
		void CreateMajorValues() {
			double alignedOffset = mapping.InternalToAligned(mapping.Offset);
			double actualOffset = mapping.Offset - mapping.AlignedToInternal(Math.Floor(alignedOffset / gridSpacing) * gridSpacing);
			double[] values;
			int index = 0;
			do {
				values = GetGridValue(gridValueProvider, index++);
				if (!AddMajorValues(actualOffset, values, index))
					break;
			} while (true);
		}
		bool AddMajorValues(double actualOffset, double[] values, int index) {
			for (int i = 0; i < values.Length; i++) {
				double value = values[i] + actualOffset;
				if (double.IsNaN(value))
					return false;
				if (value > visualRange.Max) {
					AddMajorValue(value);
					return false;
				}
				if ((value > visualRange.Min) && (items.Count == 0))
					if (value > 0) {
						double[] appendValues = GetGridValue(gridValueProvider, index - 2);
						for (int j = 0; j < appendValues.Length; j++) {
							AddMajorValue(appendValues[j] + actualOffset);
						}
					}
				AddMajorValue(value);
			}
			return true;
		}
		void CreateMinorValues() {
			if (items.Count > 0) {
				double minorBase = mapping.AlignedToInternal(Math.Floor(mapping.InternalToAligned(visualRange.Min) / gridSpacing) * gridSpacing);
				for (int i = (minorBase == items[0].Value ? 1 : 0); i < items.Count; i++) {
					double majorValue = items[i].Value;
					double minorDelta = (majorValue - minorBase) / minorStep;
					for (int j = 1; j <= minorCount + 1; j++) {
						double minorValue = minorBase + minorDelta * j;
						if (IsValueWithinRange(minorValue, visualRange.Min, visualRange.Max))
							minorValues.Add(minorValue);
					}
					minorBase = majorValue;
				}
			}
		}
		void CreateInterlacedData() {
			if (items.Count >= 2) {
				double firstValue = mapping.InternalToAligned(items.First.Value);
				int start = 1 - Math.Abs((int)(((int)(firstValue / gridSpacing) + (int)(mapping.InternalToAligned(mapping.Offset) / gridSpacing)) % 2));
				for (int i = start; i < items.Count - 1; i += 2)
					interlacedData.Add(new InterlacedData(items[i].Value, items[i + 1].Value));
				if ((start == items.Count % 2) && (items.Last.Value < visualRange.Max)) {
					double value = items.Last.Value;
					double nextValue = value + mapping.AlignedToInternal(1) * gridSpacing;
					interlacedData.Add(new InterlacedData(value, nextValue));
				}
			}
		}
		bool IsValueWithinRange(double value, double minLimit, double maxLimit) {
			return ComparingUtils.CompareDoubles(value, minLimit, epsilon) >= 0 && ComparingUtils.CompareDoubles(value, maxLimit, epsilon) <= 0;
		}
		void AddValueCore(double value, bool isAutogenerated) {
			AxisGridItem item = new AxisGridItem() { Value = value, IsAutogenerated = isAutogenerated };
			item.Visible = IsValueWithinRange(value, visualRange.Min, visualRange.Max);
			items.AddUniqueValue(item);
		}
		void AddMajorValue(double value) {
			AddValueCore(value, true);
		}
		void AddCustomValue(double value) {
			if (IsValidValue(value))
				AddValueCore(value, false);
		}
		public int GridIndex(double value) {
			return (int)GeometricUtils.StrongRound(value / mapping.AlignedToInternal(gridSpacing));
		}
		public bool ValueWithinRange(double value) {
			double minLimit, maxLimit;
			if (isRadarAxis && items.Count > 0) {
				minLimit = items.First.Value;
				maxLimit = items.Last.Value;
			}
			else {
				minLimit = visualRange.Min;
				maxLimit = visualRange.Max;
			}
			return IsValueWithinRange(value, minLimit, maxLimit);
		}
	}
}
