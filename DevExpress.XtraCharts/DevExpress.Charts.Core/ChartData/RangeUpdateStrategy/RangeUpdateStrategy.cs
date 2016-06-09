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
	public enum VisualRangeUpdateMode {
		Default,
		ProportionalFromWholeRange,
	}
	public abstract class RangeUpdateStrategy {
		protected static double defaultSideMarginValue = 0.5;
		public static void UpdateRange(IAxisData axis, MinMaxValues minMaxInternal, MinMaxValues minMaxRefined, ICollection<RefinedSeries> refinedSeries, bool needResetVisualRange, VisualRangeUpdateMode updateMode) {
			RangeUpdateStrategy strategy;
			switch (axis.AxisScaleTypeMap.ScaleType) {
				case ActualScaleType.Numerical:
					if (axis.AxisScaleTypeMap.Transformation.IsIdentity)
						strategy = new NumericalAxisRangeUpdateStrategy(axis, minMaxInternal, minMaxRefined, refinedSeries);
					else
						strategy = new NumericalLogarithmicAxisRangeUpdateStrategy(axis, minMaxInternal, minMaxRefined, refinedSeries);
					break;
				case ActualScaleType.DateTime:
					strategy = new DatetimeAxisRangeUpdateStrategy(axis, minMaxInternal, minMaxRefined, refinedSeries);
					break;
				case ActualScaleType.Qualitative:
					strategy = new QualitativeAxisRangeUpdateStrategy(axis, minMaxInternal, minMaxRefined, refinedSeries);
					break;
				default:
					return;
			}
			strategy.UpdateRange(needResetVisualRange, updateMode);
		}
		readonly IAxisData axis;
		readonly MinMaxValues minMaxInternal;
		readonly MinMaxValues minMaxRefined;
		protected readonly ICollection<RefinedSeries> refinedSeries;
		protected IAxisData Axis { get { return axis; } }
		protected IVisualAxisRangeData VisualRange { get { return axis.VisualRange; } }
		protected IWholeAxisRangeData WholeRange { get { return axis.WholeRange; } }
		protected AxisScaleTypeMap Map { get { return axis.AxisScaleTypeMap; } }
		protected bool ShowZeroLevel {
			get {
				return !axis.IsArgumentAxis && axis.WholeRange.AlwaysShowZeroLevel;
			}
		}
		protected MinMaxValues MinMaxFromSeries { get { return minMaxInternal; } }
		public RangeUpdateStrategy(IAxisData axis, MinMaxValues minMaxInternal, MinMaxValues minMaxRefined, ICollection<RefinedSeries> refinedSeries) {
			this.axis = axis;
			this.minMaxInternal = minMaxInternal;
			this.minMaxRefined = minMaxRefined;
			this.refinedSeries = refinedSeries;
		}
		void UpdateDataRange(bool needResetVisualRange, VisualRangeUpdateMode updateMode) {
			RangeSnapshot wholeSnapshot = null;
			RangeSnapshot wholeRangeNewState = null;
			RangeSnapshot visualRangeNewState = null;
			if (WholeRange != null) {
				wholeSnapshot = new RangeSnapshot(WholeRange);
				MinMaxValues wholeRangeValues = CalcWholeRangeValues();
				wholeRangeNewState = UpdateWholeRange(wholeRangeValues);
			}
			if (VisualRange != null) {
				if (VisualRange.CorrectionMode == RangeCorrectionMode.Auto) {
					visualRangeNewState = wholeRangeNewState;
					VisualRange.AutoSideMargins = WholeRange.AutoSideMargins;
				}
				else {
					visualRangeNewState = UpdateVisualRange(CalcVisualRangeValues(updateMode, CalcWholeRangeValues()));
					if (wholeSnapshot != null)
						visualRangeNewState = SynchronizeVisualRange(false, wholeRangeNewState, visualRangeNewState, updateMode);
				}
			}
			if (wholeRangeNewState != null)
				wholeRangeNewState.ApplyState(WholeRange);
			if (visualRangeNewState != null)
				visualRangeNewState.ApplyState(VisualRange);
			if (WholeRange.AutoCorrectMin)
				WholeRange.RefinedMin = minMaxRefined.Min;
			if (WholeRange.AutoCorrectMax)
				WholeRange.RefinedMax = minMaxRefined.Max;
			if (VisualRange.AutoCorrectMin)
				VisualRange.RefinedMin = minMaxRefined.Min;
			if (VisualRange.AutoCorrectMax)
				VisualRange.RefinedMax = minMaxRefined.Max;
			if (VisualRange.CorrectionMode == RangeCorrectionMode.InternalValues && WholeRange.CorrectionMode == RangeCorrectionMode.Auto && wholeRangeNewState.IsSame(visualRangeNewState)) {
				VisualRange.RefinedMin = minMaxRefined.Min;
				VisualRange.RefinedMax = minMaxRefined.Max;
			}
		}
		void UpdateRange(bool needResetVisualRange, VisualRangeUpdateMode updateMode) {
			if (Axis != null) {
				if (Axis.FixedRange)
					RefreshInternalValues();
				else
					UpdateDataRange(needResetVisualRange, updateMode);
				if (Axis.ScrollingRange != null && Axis.Range != null) {
					RangeHelper.ThrowRangeData(Axis, Axis.WholeRange, Axis.ScrollingRange);
					RangeHelper.ThrowRangeData(Axis, Axis.VisualRange, Axis.Range);
				}
			}
		}
		bool CanShowZeroLevel(MinMaxValues range) {
			if (!ShowZeroLevel)
				return false;
			double zeroLevel = GetZeroLevel(range);
			if (range.Min >= zeroLevel && WholeRange.AutoCorrectMin && VisualRange.AutoCorrectMin)
				return true;
			if (range.Max <= zeroLevel && WholeRange.AutoCorrectMax && VisualRange.AutoCorrectMax)
				return true;
			return false;
		}
		void RefreshInternalValues() {
			double min = axis.AxisScaleTypeMap.NativeToInternal(VisualRange.MinValue);
			double max = axis.AxisScaleTypeMap.NativeToInternal(VisualRange.MaxValue);
			VisualRange.UpdateRange(VisualRange.MinValue, VisualRange.MaxValue, min, max);
			min = axis.AxisScaleTypeMap.NativeToInternal(WholeRange.MinValue);
			max = axis.AxisScaleTypeMap.NativeToInternal(WholeRange.MaxValue);
			WholeRange.UpdateRange(WholeRange.MinValue, WholeRange.MaxValue, min, max);
		}
		RangeSnapshot SynchronizeVisualRange(bool needSyncWhisWholeRange, RangeSnapshot wholeRangeNewState, RangeSnapshot visualRangeNewState, VisualRangeUpdateMode updateMode) {
			RangeSnapshot snapshot = visualRangeNewState;
			if (VisualRange.CorrectionMode == RangeCorrectionMode.InternalValues && updateMode == VisualRangeUpdateMode.ProportionalFromWholeRange && !wholeRangeNewState.IsSame(visualRangeNewState))
				snapshot = ProportionalCorrectVisualRange(wholeRangeNewState, visualRangeNewState);
			return SynchronizeVisualRange(wholeRangeNewState, snapshot, needSyncWhisWholeRange);
		}
		RangeSnapshot ProportionalCorrectVisualRange(RangeSnapshot wholeRangeNewState, RangeSnapshot visualRangeNewState) {
			if (visualRangeNewState.Min == wholeRangeNewState.Min && visualRangeNewState.Max == wholeRangeNewState.Max)
				return visualRangeNewState;
			IMinMaxValues wholeRange = wholeRangeNewState as IMinMaxValues;
			IMinMaxValues visualRange;
			if (double.IsNaN(VisualRange.Min) || double.IsNaN(VisualRange.Min))
				visualRange = visualRangeNewState as IMinMaxValues;
			else
				visualRange = VisualRange as IMinMaxValues;
			double minOffset = ((visualRange.Min - WholeRange.Min) / WholeRange.Delta) * wholeRange.Delta;
			double maxOffset = ((visualRange.Max - WholeRange.Min) / WholeRange.Delta) * wholeRange.Delta;
			double min;
			double max;
			if (axis.AxisScaleTypeMap.Transformation.IsIdentity) {
				min = wholeRange.Min + minOffset;
				max = wholeRange.Min + maxOffset;
			}
			else {
				min = Math.Min(visualRangeNewState.Min, wholeRange.Min + minOffset);
				max = Math.Max(visualRangeNewState.Max, wholeRange.Min + maxOffset);
			}
			InternalRange internalRange = new InternalRange();
			internalRange.Values = new MinMaxValues(min, max);
			internalRange.SideMargins = new MinMaxValues(0);
			return new RangeSnapshot(InternalToNative(internalRange.Values), internalRange);
		}
		bool CheckValueByMap(double value) {
			if (Map.ScaleType == ActualScaleType.Qualitative)
				return value >= 0;
			return true;
		}
		RangeSnapshot SynchronizeVisualRange(RangeSnapshot wholeSnapshot, RangeSnapshot visualSnapshot, bool needSyncWhisWholeRange) {
			if (VisualRange.SynchronizeWithWholeRange && needSyncWhisWholeRange)
				return wholeSnapshot;
			else {
				visualSnapshot.Adjust(wholeSnapshot, axis.AxisScaleTypeMap);
				return visualSnapshot;
			}
		}
		MinMaxValues CalcWholeRangeValues() {
			double actualMin = double.NaN;
			double actualMax = double.NaN;
			switch (WholeRange.CorrectionMode) {
				case RangeCorrectionMode.Auto:
					return minMaxInternal;
				case RangeCorrectionMode.InternalValues:
					actualMin = double.NaN;
					actualMax = double.NaN;
					if (WholeRange.AutoCorrectMin && minMaxInternal.HasValues && !minMaxInternal.IsEmpty)
						actualMin = minMaxInternal.Min;
					else
						actualMin = (!double.IsNaN(WholeRange.Min)) ? WholeRange.Min + WholeRange.SideMarginsMin : Map.NativeToInternal(WholeRange.MinValue);
					if (WholeRange.AutoCorrectMax && minMaxInternal.HasValues && !minMaxInternal.IsEmpty)
						actualMax = minMaxInternal.Max;
					else
						actualMax = (!double.IsNaN(WholeRange.Max)) ? WholeRange.Max + WholeRange.SideMarginsMax : Map.NativeToInternal(WholeRange.MaxValue);
					return new MinMaxValues(actualMin, actualMax);
				default:
				case RangeCorrectionMode.Values:
					return CalcRangeValuesInValuesMode(WholeRange, minMaxInternal);
			}
		}
		MinMaxValues CalcVisualRangeValues(VisualRangeUpdateMode updateMode, MinMaxValues defaultMinMax) {
			RangeCorrectionMode correctionMode;
			if (VisualRange.CorrectionMode == RangeCorrectionMode.Auto && (double.IsNaN(minMaxInternal.Max) || double.IsNaN(minMaxInternal.Min)))
				correctionMode = RangeCorrectionMode.Values;
			else
				correctionMode = VisualRange.CorrectionMode;
			switch (correctionMode) {
				case RangeCorrectionMode.Auto:
					return minMaxInternal;
				case RangeCorrectionMode.InternalValues:
					double min = (updateMode == VisualRangeUpdateMode.Default) || (VisualRange.MinValue == null) ? VisualRange.Min : Map.NativeToInternal(VisualRange.MinValue);
					double max = (updateMode == VisualRangeUpdateMode.Default) || (VisualRange.MaxValue == null) ? VisualRange.Max : Map.NativeToInternal(VisualRange.MaxValue);
					return new MinMaxValues(min + VisualRange.SideMarginsMin, max - VisualRange.SideMarginsMax);
				default:
				case RangeCorrectionMode.Values:
					return CalcRangeValuesInValuesMode(VisualRange, defaultMinMax);
			}
		}
		InternalRange PrepareCorrectedInternalValue(MinMaxValues correctedValues, MinMaxValues internalValues, double sideMargin) {
			InternalRange range;
			range = new InternalRange();
			range.Values = correctedValues;
			range.SideMargins = new MinMaxValues(internalValues.Min - correctedValues.Min, correctedValues.Max - internalValues.Max);
			range.SideMarginsValue = sideMargin;
			return range;
		}
		MinMaxValues ApplySideMargin(MinMaxValues values, double sideMargin) {
			MinMaxValues newValues = new MinMaxValues();
			if (sideMargin != 0) {
				newValues.Min = values.Min - sideMargin;
				newValues.Max = values.Max + sideMargin;
			}
			else {
				newValues.Min = values.Min - defaultSideMarginValue;
				newValues.Max = values.Max + defaultSideMarginValue;
			}
			return newValues;
		}
		protected abstract RangeSnapshot UpdateVisualRange(MinMaxValues visualRangeValues);
		protected virtual MinMaxValues CalcRangeValuesInValuesMode(IAxisRangeData range, MinMaxValues defaultMinMax) {
			double actualMin = double.NaN;
			double actualMax = double.NaN;
			if (range.AutoCorrectMin) {
				if (!defaultMinMax.HasValues || defaultMinMax.IsEmpty)
					actualMin = double.NaN;
				else
					actualMin = defaultMinMax.Min;
			}
			else {
				if (range.MinValue == null)
					actualMin = range.Min + range.SideMarginsMin;
				else
					actualMin = Map.NativeToInternal(range.MinValue);
			}
			if (range.AutoCorrectMax) {
				if (!defaultMinMax.HasValues || defaultMinMax.IsEmpty)
					actualMax = double.NaN;
				else
					actualMax = defaultMinMax.Max;
			}
			else {
				if (range.MaxValue == null)
					actualMax = range.Max + range.SideMarginsMax;
				else
					actualMax = Map.NativeToInternal(range.MaxValue);
			}
			return new MinMaxValues(actualMin, actualMax);
		}
		protected virtual double CalcSideMargin(MinMaxValues internalValues, IAxisRangeData range) {
			return SideMarginCalculator.CalcSideMargin(Axis, internalValues, range.AutoCorrectMin, range.AutoCorrectMax, refinedSeries);
		}
		protected virtual RangeSnapshot DefaultRange(IAxisRangeData range) {
			InternalRange internalRange = new InternalRange();
			internalRange.Values = new MinMaxValues(0, 1);
			internalRange.SideMargins = new MinMaxValues(0, 0);
			NativeRange nativeRange = InternalToNative(internalRange.Values);
			internalRange.SideMarginsValue = range.SideMarginsValue;
			RangeSnapshot rangeSnapshot = new RangeSnapshot(nativeRange, internalRange);
			return rangeSnapshot;
		}
		protected virtual NativeRange CorrectNativeValues(MinMaxValues internalValues, MinMaxValues corectedInternalValues, IAxisRangeData range) {
			switch (Map.ScaleType) {
				case ActualScaleType.Numerical:
					return InternalToNative(corectedInternalValues);
				case ActualScaleType.DateTime:
					if (internalValues.Min == internalValues.Max) {
						DateTime date = (DateTime)Map.InternalToNative(internalValues.Min);
						if (internalValues.Min != 0)
							return new NativeRange(date.AddDays(-1), date.AddDays(1));
						else
							return new NativeRange(date, date.AddDays(1));
					}
					break;
				case ActualScaleType.Qualitative:
					if ((double.IsNaN(internalValues.Min) && double.IsNaN(internalValues.Max)) || internalValues.Min == internalValues.Max)
						return new NativeRange("Min", "Max");
					object minValue = Map.InternalToNative(internalValues.Min);
					if (internalValues.Min == -1)
						minValue = WholeRange.MinValue;
					object maxValue = Map.InternalToNative(internalValues.Max);
					if (internalValues.Max == -1)
						maxValue = WholeRange.MaxValue;
					return new NativeRange(minValue, maxValue);
			}
			return InternalToNative(internalValues);
		}
		protected virtual InternalRange CorrectInternalValue(MinMaxValues internalValues, double sideMargin, bool autoCorrectionMin, bool autoCorrectionMax) {
			if (double.IsNaN(internalValues.Min) && double.IsNaN(internalValues.Max)) {
				InternalRange range = new InternalRange();
				range.Values = new MinMaxValues(0, 1.0);
				range.SideMargins = new MinMaxValues(0, 0);
				return range;
			}
			MinMaxValues newValues = new MinMaxValues(0);
			if (Axis.IsRadarAxis) {
				newValues = CheckZeroLevel(internalValues, internalValues, autoCorrectionMin, autoCorrectionMax);
				if (newValues.Min == newValues.Max)
					newValues.Max += 1;
				return PrepareCorrectedInternalValue(newValues, internalValues, sideMargin);
			}
			if (internalValues.Min < internalValues.Max) {
				newValues.Min = internalValues.Min - sideMargin;
				newValues.Max = internalValues.Max + sideMargin;
				newValues = CheckZeroLevel(newValues, internalValues, autoCorrectionMin, autoCorrectionMax);
			}
			if (internalValues.Min == internalValues.Max) {
				newValues.Min = internalValues.Min - sideMargin;
				newValues.Max = internalValues.Max + sideMargin;
				if (internalValues.Min != 0)
					newValues = CheckZeroLevel(newValues, internalValues, autoCorrectionMin, autoCorrectionMax);
				if (newValues.Min == newValues.Max) {
					newValues.Min = newValues.Min - defaultSideMarginValue;
					newValues.Max = newValues.Max + defaultSideMarginValue;
				}
			}
			if (internalValues.Min > internalValues.Max) {
				if (!autoCorrectionMin && autoCorrectionMax) {
					newValues.Min = internalValues.Min;
					newValues.Max = newValues.Min;
				}
				if (!autoCorrectionMax && autoCorrectionMin) {
					newValues.Max = internalValues.Max;
					newValues.Min = newValues.Max;
				}
				if (!autoCorrectionMin && !autoCorrectionMax) {
					newValues.Min = internalValues.Max;
					newValues.Max = internalValues.Min;
				}
				newValues = ApplySideMargin(newValues, sideMargin);
			}
			if (double.IsNaN(internalValues.Min) || double.IsNaN(internalValues.Max)) {
				if (double.IsNaN(internalValues.Min)) {
					newValues.Min = internalValues.Max;
					newValues.Max = internalValues.Max;
				}
				if (double.IsNaN(internalValues.Max)) {
					newValues.Max = internalValues.Min;
					newValues.Min = internalValues.Min;
				}
				newValues = ApplySideMargin(newValues, sideMargin);
			}
			return PrepareCorrectedInternalValue(newValues, internalValues, sideMargin);
		}
		protected virtual InternalRange CorrectWithSideMargins(IAxisRangeData range, MinMaxValues internalValues) {
			double sideMargin = range.SideMarginsValue;
			if (AutoSideMarginEnable(range) || (IncorrectRelationBetweenMinAndMax(internalValues) && sideMargin == 0)) {
				if (range is IWholeAxisRangeData && WholeRange.CorrectionMode == RangeCorrectionMode.InternalValues && VisualRange.MinValue != null && VisualRange.MaxValue != null) {
					MinMaxValues vrange = new MinMaxValues(Axis.AxisScaleTypeMap.NativeToInternal(VisualRange.MinValue), Axis.AxisScaleTypeMap.NativeToInternal(VisualRange.MaxValue));
					sideMargin = CalcSideMargin(vrange, range);
				}
				else
					sideMargin = CalcSideMargin(internalValues, range);
			}
			InternalRange correctedRange = CorrectInternalValue(internalValues, sideMargin, range.AutoCorrectMin, range.AutoCorrectMax);
			if (AutoSideMarginEnable(range))
				correctedRange.SideMarginsValue = sideMargin;
			else
				correctedRange.SideMarginsValue = range.SideMarginsValue;
			return correctedRange;
		}
		protected virtual InternalRange CheckInternalValue(IAxisRangeData range, MinMaxValues internalValues) {
			InternalRange corectedInternalValues = new InternalRange();
			double min = internalValues.Min - range.SideMarginsMin;
			double max = internalValues.Max + range.SideMarginsMax;
			if (double.IsNaN(min))
				min = 0;
			if (double.IsNaN(max))
				max = 1;
			if (min < 0) {
				max += -min;
				min = 0;
			}
			if (min >= max) {
				if (range.AutoCorrectMin)
					min = max - defaultSideMarginValue;
				if (range.AutoCorrectMax)
					max = min + defaultSideMarginValue;
			}
			corectedInternalValues.Values = new MinMaxValues(min, max);
			corectedInternalValues.SetSideMargins(range);
			return corectedInternalValues;
		}
		protected virtual RangeSnapshot UpdateWholeRange(MinMaxValues internalValues) {
			if (!internalValues.HasValues) {
				return DefaultRange(WholeRange);
			}
			else {
				InternalRange corectedInternalValues = new InternalRange();
				if (NeedCorrect(WholeRange))
					corectedInternalValues = CorrectWithSideMargins(WholeRange, internalValues);
				else
					corectedInternalValues = CheckInternalValue(WholeRange, internalValues);
				NativeRange nativeValues = CorrectNativeValues(internalValues, corectedInternalValues.Values, WholeRange);
				if (nativeValues.Min == null && nativeValues.Max == null)
					return DefaultRange(WholeRange);
				return new RangeSnapshot(nativeValues, corectedInternalValues);
			}
		}
		protected virtual bool NeedCorrect(IAxisRangeData range) {
			return (range.CorrectionMode == RangeCorrectionMode.Auto || range.CorrectionMode == RangeCorrectionMode.Values) &&
				(!(Axis.IsRadarAxis && Axis.IsArgumentAxis) || (range.AlwaysShowZeroLevel && range.CorrectionMode == RangeCorrectionMode.Auto));
		}
		protected virtual bool IncorrectRelationBetweenMinAndMax(MinMaxValues internalValues) {
			return (internalValues.Max <= internalValues.Min && !CanShowZeroLevel(internalValues));
		}
		protected double GetZeroLevel(MinMaxValues internalValues) {
			if (Axis.AxisScaleTypeMap.ScaleType == ActualScaleType.DateTime)
				return internalValues.Min;
			return 0;
		}
		protected NativeRange InternalToNative(MinMaxValues range) {
			object userMinValue = Map.InternalToNative(range.Min);
			object userMaxValue = Map.InternalToNative(range.Max);
			return new NativeRange(userMinValue, userMaxValue);
		}
		protected MinMaxValues CheckZeroLevel(MinMaxValues newValues, MinMaxValues internalValues, bool autoCorrectionMin, bool autoCorrectionMax) {
			if (ShowZeroLevel) {
				double zeroLevel = GetZeroLevel(internalValues);
				if (internalValues.Min >= zeroLevel && autoCorrectionMin && newValues.Max != 0)
					newValues.Min = zeroLevel;
				if (internalValues.Max <= zeroLevel && autoCorrectionMax && newValues.Min != 0)
					newValues.Max = zeroLevel;
			}
			return newValues;
		}
		protected bool AutoSideMarginEnable(IAxisRangeData range) { return range.AutoSideMargins == SideMarginMode.Enable || range.AutoSideMargins == SideMarginMode.UserEnable; }
	}
	public class RangeSnapshot : IMinMaxValues {
		object min;
		object max;
		double internalMin;
		double internalMax;
		double sideMarginMin;
		double sideMarginMax;
		double sideMarginsValue;
		public object MinValue { get { return min; } }
		public object MaxValue { get { return max; } }
		public double Min { get { return internalMin; } }
		public double Max { get { return internalMax; } }
		public RangeSnapshot(IAxisRangeData range) {
			min = range.MinValue;
			max = range.MaxValue;
			internalMin = range.Min;
			internalMax = range.Max;
			sideMarginMin = range.SideMarginsMin;
			sideMarginMax = range.SideMarginsMax;
			sideMarginsValue = range.SideMarginsValue;
		}
		public RangeSnapshot(NativeRange range, InternalRange internalRange) {
			min = range.Min;
			max = range.Max;
			internalMin = internalRange.Values.Min;
			internalMax = internalRange.Values.Max;
			sideMarginMin = internalRange.SideMargins.Min;
			sideMarginMax = internalRange.SideMargins.Max;
			sideMarginsValue = internalRange.SideMarginsValue;
		}
		#region IMinMaxValues Members
		double IMinMaxValues.Min {
			get {
				return internalMin;
			}
			set {
				internalMin = value;
			}
		}
		double IMinMaxValues.Max {
			get {
				return internalMax;
			}
			set {
				internalMax = value;
			}
		}
		double IMinMaxValues.Delta { get { return internalMax - internalMin; } }
		double IMinMaxValues.CalculateCenter() {
			return MinMaxValues.CalculateCenter(this);
		}
		void IMinMaxValues.Intersection(IMinMaxValues minMaxValues) {
			MinMaxValues.Intersection(this, minMaxValues);
		}
		#endregion
		void AdjustNativValue(RangeSnapshot bounds, AxisScaleTypeMap map) {
			if (map.NativeToInternal(max) > map.NativeToInternal(bounds.max) || map.NativeToInternal(max) < map.NativeToInternal(bounds.min)) {
				max = bounds.max;
				internalMax = bounds.internalMax;
			}
			if (map.NativeToInternal(min) < map.NativeToInternal(bounds.min) || map.NativeToInternal(min) > map.NativeToInternal(bounds.max)) {
				min = bounds.min;
				internalMin = bounds.internalMin;
			}
		}
		void AdjustInternalValue(RangeSnapshot bounds) {
			if (internalMax > bounds.internalMax || internalMax < bounds.internalMin) {
				max = bounds.max;
				internalMax = bounds.internalMax;
			}
			if (internalMin < bounds.internalMin || internalMin > bounds.internalMax) {
				min = bounds.min;
				internalMin = bounds.internalMin;
			}
		}
		public bool IsSame(IAxisRangeData range) {
			if (range == null)
				return false;
			return range.MinValue.Equals(min) && range.MaxValue.Equals(max) && range.Max == internalMax && range.Min == internalMin;
		}
		public bool IsSame(RangeSnapshot range) {
			if (range == null)
				return false;
			if ((min == null ^ range.min == null) || (max == null ^ range.max == null))
				return false;
			if (min != null && range.min != null && max != null && range.max != null)
				return range.min.Equals(min) && range.max.Equals(max) && range.internalMax == internalMax && range.internalMin == internalMin;
			return range.internalMax == internalMax && range.internalMin == internalMin;
		}
		public void Adjust(RangeSnapshot bounds, AxisScaleTypeMap map) {
			AdjustInternalValue(bounds);
			if (internalMin == internalMax) {
				internalMax = bounds.internalMax;
				internalMin = bounds.internalMin;
			}
		}
		public void ApplyState(IAxisRangeData range) {
			range.SideMarginsMin = sideMarginMin;
			range.SideMarginsMax = sideMarginMax;
			range.SideMarginsValue = sideMarginsValue;
			range.UpdateRange(min, max, internalMin, internalMax);
		}
		public bool IsSubsetOf(RangeSnapshot range) {
			if (range == null)
				return false;
			if ((min == null ^ range.min == null) || (max == null ^ range.max == null))
				return false;
			return range.internalMax >= internalMax && range.internalMin <= internalMin;
		}
	}
	public struct NativeRange {
		public object Min;
		public object Max;
		public NativeRange(object min, object max) {
			Min = min;
			Max = max;
		}
	}
	public class InternalRange {
		public MinMaxValues ValuesWithoutSideMargins { get { return new MinMaxValues(Values.Min + SideMargins.Min, Values.Max - SideMargins.Max); } }
		public MinMaxValues Values;
		public MinMaxValues SideMargins;
		public double SideMarginsValue;
		public InternalRange() {
			SideMargins = new MinMaxValues(0);
			SideMarginsValue = 0;
		}
		public void SetSideMargins(IAxisRangeData range) {
			SideMargins = new MinMaxValues(range.SideMarginsMin, range.SideMarginsMax);
			SideMarginsValue = range.SideMarginsValue;
		}
	}
}
