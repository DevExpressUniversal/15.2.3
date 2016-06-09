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
	public class QualitativeAxisRangeUpdateStrategy : RangeUpdateStrategy {
		public QualitativeAxisRangeUpdateStrategy(IAxisData axis, MinMaxValues minMaxInternal, MinMaxValues minMaxRefined, ICollection<RefinedSeries> refinedSeries)
			: base(axis, minMaxInternal, minMaxRefined, refinedSeries) { }
		protected override RangeSnapshot UpdateWholeRange(MinMaxValues internalValues) {
			InternalRange corectedInternalValues;
			if (!internalValues.HasValues && (!MinMaxFromSeries.HasValues || MinMaxFromSeries.Delta == 0)) {
				return DefaultRange(VisualRange);
			}
			if (IsAxisContainsValues(internalValues)) {
				if (NeedCorrect(WholeRange))
					corectedInternalValues = CorrectWithSideMargins(WholeRange, internalValues);
				else {
					corectedInternalValues = new InternalRange();
					corectedInternalValues.Values = new MinMaxValues(internalValues.Min - WholeRange.SideMarginsMin, internalValues.Max + WholeRange.SideMarginsMax);
					corectedInternalValues.SetSideMargins(WholeRange);
				}
			}
			else {
				corectedInternalValues = new InternalRange();
				corectedInternalValues.SideMargins = new MinMaxValues(defaultSideMarginValue, defaultSideMarginValue);
				corectedInternalValues.Values = new MinMaxValues(0);
				corectedInternalValues.Values.Min = double.IsNaN(internalValues.Min) ? MinMaxFromSeries.Min - defaultSideMarginValue : internalValues.Min - defaultSideMarginValue;
				corectedInternalValues.Values.Max = double.IsNaN(internalValues.Max) ? MinMaxFromSeries.Max + defaultSideMarginValue : internalValues.Max + defaultSideMarginValue;
			}
			NativeRange nativeValues;
			nativeValues = CorrectNativeValues(internalValues, corectedInternalValues.Values, WholeRange);
			if (nativeValues.Min == null || nativeValues.Max == null)
				return DefaultRange(VisualRange);
			else
				return new RangeSnapshot(nativeValues, corectedInternalValues);
		}
		protected override RangeSnapshot UpdateVisualRange(MinMaxValues visualRangeValues) {
			InternalRange corectedInternalValues;
			if (!visualRangeValues.HasValues) {
				return DefaultRange(VisualRange);
			}
			else {
				if (NeedCorrect(VisualRange))
					corectedInternalValues = CorrectWithSideMargins(VisualRange, visualRangeValues);
				else {
					corectedInternalValues = CheckInternalValue(VisualRange, visualRangeValues);
				}
				NativeRange nativeValues = RestoreValuesFromVisualRange(corectedInternalValues);
				return new RangeSnapshot(nativeValues, corectedInternalValues);
			}
		}
		protected override InternalRange CorrectInternalValue(MinMaxValues internalValues, double sideMargin, bool autoCorrectionMin, bool autoCorrectionMax) {
			InternalRange corectedInternalValues = base.CorrectInternalValue(internalValues, sideMargin, autoCorrectionMin, autoCorrectionMax);
			if (corectedInternalValues.Values.Max == corectedInternalValues.Values.Min) {
				corectedInternalValues.Values = new MinMaxValues(internalValues.Min - defaultSideMarginValue, internalValues.Max + defaultSideMarginValue);
				corectedInternalValues.SideMargins = new MinMaxValues(defaultSideMarginValue, defaultSideMarginValue);
			}
			return corectedInternalValues;
		}
		protected override NativeRange CorrectNativeValues(MinMaxValues internalValues, MinMaxValues corectedInternalValues, IAxisRangeData range) {
			if (!internalValues.HasValues && !corectedInternalValues.HasValues || (internalValues.Min == internalValues.Max && range.CorrectionMode == RangeCorrectionMode.Auto))
				return new NativeRange("Min", "Max");
			object minValue;
			object maxValue;
			switch (range.CorrectionMode) {
				case RangeCorrectionMode.Auto:
				default:
					minValue = double.IsNaN(internalValues.Min) ? (range.MinValue) : Map.InternalToNative(internalValues.Min);
					maxValue = double.IsNaN(internalValues.Max) ? (range.MaxValue) : Map.InternalToNative(internalValues.Max);
					return new NativeRange(minValue, maxValue);
				case RangeCorrectionMode.Values:
					return new NativeRange(range.MinValue, range.MaxValue);
				case RangeCorrectionMode.InternalValues:
					double min = 0;
					if (range.MinValue != null)
						min = Map.NativeToInternal(range.MinValue);
					else
						min = range.Min;
					double max = 1;
					if (range.MaxValue != null)
						max = Map.NativeToInternal(range.MaxValue);
					else
						max = range.Max;
					minValue = double.IsNaN(min) ? (range.MinValue) : Map.InternalToNative(internalValues.Min);
					maxValue = double.IsNaN(max) ? (range.MaxValue) : Map.InternalToNative(internalValues.Max);
					return new NativeRange(minValue, maxValue);
			}
		}
		protected override RangeSnapshot DefaultRange(IAxisRangeData range) {
			NativeRange nativeRange = new NativeRange("Min", "Max");
			InternalRange internalRange = new InternalRange();
			internalRange.Values = new MinMaxValues(0, 1);
			internalRange.SideMargins = new MinMaxValues(0, 0);
			internalRange.SideMarginsValue = range.SideMarginsValue;
			RangeSnapshot rangeSnapshot = new RangeSnapshot(nativeRange, internalRange);
			return rangeSnapshot;
		}
		protected override bool IncorrectRelationBetweenMinAndMax(MinMaxValues internalValues) {
			return (internalValues.Max <= internalValues.Min);
		}
		protected override InternalRange CheckInternalValue(IAxisRangeData range, MinMaxValues internalValues) {
			InternalRange corectedInternalValues = new InternalRange();
			if (double.IsNaN(internalValues.Min))
				corectedInternalValues.Values.Min = internalValues.Max - defaultSideMarginValue;
			else
				corectedInternalValues.Values.Min = internalValues.Min - VisualRange.SideMarginsMin;
			if (double.IsNaN(internalValues.Max))
				corectedInternalValues.Values.Max = internalValues.Min + defaultSideMarginValue;
			else
				corectedInternalValues.Values.Max = internalValues.Max + VisualRange.SideMarginsMax;
			corectedInternalValues.SideMargins = new MinMaxValues(VisualRange.SideMarginsMin, VisualRange.SideMarginsMax);
			corectedInternalValues.SideMarginsValue = VisualRange.SideMarginsValue;
			return corectedInternalValues;
		}
		protected override MinMaxValues CalcRangeValuesInValuesMode(IAxisRangeData range, MinMaxValues defaultMinMax) {
			MinMaxValues value = base.CalcRangeValuesInValuesMode(range, defaultMinMax);
			double actualMin = value.Min;
			double actualMax = value.Max;
			if (Map.ScaleType == ActualScaleType.Qualitative) {
				AxisQualitativeMap qualitativeMap = Map as AxisQualitativeMap;
				if (double.IsNaN(actualMin))
					actualMin = 0;
				if (double.IsNaN(actualMax))
					actualMax = Math.Max(1, qualitativeMap.UniqueValuesCount - 1);
			}
			return new MinMaxValues(actualMin, actualMax);
		}
		NativeRange RestoreValuesFromVisualRange(InternalRange internalValues) {
			if (VisualRange.CorrectionMode == RangeCorrectionMode.Auto && internalValues.ValuesWithoutSideMargins.Min == 0 && internalValues.ValuesWithoutSideMargins.Max == 0)
				return new NativeRange("Min", "Max");
			object newMin = VisualRange.MinValue;
			object newMax = VisualRange.MaxValue;
			if (VisualRange.MinValue == null && !double.IsNaN(internalValues.ValuesWithoutSideMargins.Min) || VisualRange.AutoCorrectMin)
				newMin = Map.InternalToNative(internalValues.ValuesWithoutSideMargins.Min);
			if (VisualRange.MaxValue == null && !double.IsNaN(internalValues.ValuesWithoutSideMargins.Max) || VisualRange.AutoCorrectMax)
				newMax = Map.InternalToNative(internalValues.ValuesWithoutSideMargins.Max);
			return new NativeRange(newMin, newMax);
		}
		bool IsAxisContainsValues(MinMaxValues internalValues) {
			return internalValues.Min > -1 && internalValues.Max > -1;
		}
	}
}
