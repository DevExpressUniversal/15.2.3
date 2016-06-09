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
	public class NumericalAxisRangeUpdateStrategy : RangeUpdateStrategy {
		public NumericalAxisRangeUpdateStrategy(IAxisData axis, MinMaxValues minMaxInternal, MinMaxValues minMaxRefined, ICollection<RefinedSeries> refinedSeries)
			: base(axis, minMaxInternal, minMaxRefined, refinedSeries) { }
		protected override RangeSnapshot UpdateVisualRange(MinMaxValues visualRangeValues) {
			if (!visualRangeValues.HasValues) {
				return DefaultRange(VisualRange);
			}
			else {
				InternalRange corectedInternalValues;
				if (NeedCorrect(VisualRange))
					corectedInternalValues = CorrectWithSideMargins(VisualRange, visualRangeValues);
				else {
					corectedInternalValues = new InternalRange();
					corectedInternalValues.Values = visualRangeValues;
					corectedInternalValues.SideMargins = new MinMaxValues(0, 0);
					corectedInternalValues = CheckInternalValue(VisualRange, corectedInternalValues.Values);
				}
				NativeRange nativeValues = CorrectNativeValues(visualRangeValues, corectedInternalValues.Values, VisualRange);
				return new RangeSnapshot(nativeValues, corectedInternalValues);
			}
		}
		protected override NativeRange CorrectNativeValues(MinMaxValues internalValues, MinMaxValues corectedInternalValues, IAxisRangeData range) {
			if (Axis.IsArgumentAxis) {
				if (double.IsNaN(internalValues.Max) || double.IsNaN(internalValues.Min)) {
					MinMaxValues values = new MinMaxValues();
					if (double.IsNaN(internalValues.Max)) {
						values.Min = internalValues.Min;
						values.Max = internalValues.Min;
					}
					else if (double.IsNaN(internalValues.Min)) {
						values.Min = internalValues.Max;
						values.Max = internalValues.Max;
					}
					return InternalToNative(values);
				}
				return InternalToNative(internalValues);
			}
			if (internalValues.Min > internalValues.Max) {
				double min = range.AutoCorrectMin ? corectedInternalValues.Min : internalValues.Min;
				double max = range.AutoCorrectMax ? corectedInternalValues.Max : internalValues.Max;
				return InternalToNative(new MinMaxValues(min, max));
			}
			if (range.CorrectionMode == RangeCorrectionMode.InternalValues) {
				return InternalToNative(internalValues);
			}
			else {
				if (ShowZeroLevel) {
					MinMaxValues newValues = CheckZeroLevel(internalValues, internalValues, range.AutoCorrectMin, range.AutoCorrectMax);
					return InternalToNative(newValues);
				}
				return InternalToNative(internalValues);
			}
		}
		protected override InternalRange CheckInternalValue(IAxisRangeData range, MinMaxValues internalValues) {
			double min = internalValues.Min;
			double max = internalValues.Max;
			if (min >= max) {
				if (range.AutoCorrectMin)
					min = max - defaultSideMarginValue;
				if (range.AutoCorrectMax)
					max = min + defaultSideMarginValue;
				if (min == max) {
					min = min - defaultSideMarginValue;
					max = max + defaultSideMarginValue;
				}
			}
			InternalRange corectedInternalValues = new InternalRange();
			corectedInternalValues.Values = new MinMaxValues(min - (range.AutoCorrectMin ? range.SideMarginsMin : 0), max + (range.AutoCorrectMax ? range.SideMarginsMax : 0));
			corectedInternalValues.SideMargins = new MinMaxValues(range.SideMarginsMin, range.SideMarginsMax);
			return corectedInternalValues;
		}
	}
	public class NumericalLogarithmicAxisRangeUpdateStrategy : NumericalAxisRangeUpdateStrategy {
		const double sideMargins = 0.5;
		public NumericalLogarithmicAxisRangeUpdateStrategy(IAxisData axis, MinMaxValues minMaxInternal, MinMaxValues minMaxRefined, ICollection<RefinedSeries> refinedSeries)
			: base(axis, minMaxInternal, minMaxRefined, refinedSeries) { }
		bool IsRadarSeries() {
			if (refinedSeries != null)
				foreach (IRefinedSeries series in refinedSeries)
					if (series.SeriesView.CompatibleViewType == CompatibleViewType.PolarView || series.SeriesView.CompatibleViewType == CompatibleViewType.RadarView)
						return true;
			return false;
		}
		protected override double CalcSideMargin(MinMaxValues internalValues, IAxisRangeData range) {
			return SideMarginLogarithmicCalculator.CalcSideMargin(Axis, internalValues, this.refinedSeries);
		}
		protected override InternalRange CorrectInternalValue(MinMaxValues internalValues, double sideMargin, bool autoCorrectionMin, bool autoCorrectionMax) {
			double newMin = 0;
			double newMax = 0;
			if (internalValues.Min == 0 && internalValues.Max == 0) {
				newMin = 0;
				newMax = 1;
			}
			else {
				Transformation transformation = Axis.AxisScaleTypeMap.Transformation;
				if (sideMargin > 0) {
					if (IsRadarSeries()) {
						newMin = transformation.TransformBackward(transformation.TransformForward(internalValues.Min) - sideMargin);
						newMax = transformation.TransformBackward(transformation.TransformForward(internalValues.Max) + sideMargin);
					}
					else {
						double minTransform = transformation.TransformForward(internalValues.Min);
						double maxTransform = transformation.TransformForward(internalValues.Max);
						newMin = transformation.TransformBackward(minTransform - sideMargin);
						newMax = transformation.TransformBackward(maxTransform + sideMargin);
						LogarithmicTransformation logarithmicTransformation = Axis.AxisScaleTypeMap.Transformation as LogarithmicTransformation;
						if (logarithmicTransformation != null) {
							if (internalValues.Min >= 0 && internalValues.Max > 0 && newMin < 0)
								newMin = RoundLogarithmicValue(internalValues.Min, logarithmicTransformation, -1);
							if (internalValues.Min < 0 && internalValues.Max <= 0 && newMax > 0)
								newMax = RoundLogarithmicValue(internalValues.Max, logarithmicTransformation, 1);
						}
					}
				}
				else {
					newMin = internalValues.Min;
					newMax = internalValues.Max;
				}
				if (ShowZeroLevel) {
					double zeroLevel = 0;
					if (internalValues.Min >= zeroLevel && autoCorrectionMin)
						newMin = zeroLevel;
					if (internalValues.Max <= zeroLevel && autoCorrectionMax)
						newMax = zeroLevel;
				}
			}
			if (newMin == 0 && newMax == 0) {
				newMin = 0;
				newMax = 1;
			}
			InternalRange range = new InternalRange();
			range.Values = new MinMaxValues(newMin, newMax);
			range.SideMargins = new MinMaxValues(internalValues.Min - newMin, newMax - internalValues.Max);
			return range;
		}
		double RoundLogarithmicValue(double value, LogarithmicTransformation logarithmicTransformation, double powerOffset) {
			double min = Math.Log(Math.Abs(value), logarithmicTransformation.LogarithmicBase);
			if (min - Math.Round(min) > 0.5)
				min = Math.Ceiling(min);
			else
				min = Math.Floor(min);
			min += powerOffset;
			return Math.Pow(logarithmicTransformation.LogarithmicBase, min);
		}
		protected override NativeRange CorrectNativeValues(MinMaxValues internalValues, MinMaxValues corectedInternalValues, IAxisRangeData range) {
			if (Axis.IsArgumentAxis)
				return InternalToNative(internalValues);
			if (range.CorrectionMode == RangeCorrectionMode.Values || range.CorrectionMode == RangeCorrectionMode.InternalValues) {
				return InternalToNative(internalValues);
			}
			else {
				MinMaxValues newValues = CheckZeroLevel(internalValues.Clone(), internalValues, range.AutoCorrectMin, range.AutoCorrectMin);
				return InternalToNative(newValues);
			}
		}
	}
}
