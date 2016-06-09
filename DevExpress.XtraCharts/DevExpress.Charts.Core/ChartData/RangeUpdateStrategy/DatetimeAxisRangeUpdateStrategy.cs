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
	public class DatetimeAxisRangeUpdateStrategy : RangeUpdateStrategy {
		public DatetimeAxisRangeUpdateStrategy(IAxisData axis, MinMaxValues minMaxInternal, MinMaxValues minMaxRefined, ICollection<RefinedSeries> refinedSeries)
			: base(axis, minMaxInternal, minMaxRefined, refinedSeries) { }
		TimeSpan GetMeasureUnitTimeSpan() {
			switch (Axis.DateTimeScaleOptions.MeasureUnit) {
				case DateTimeMeasureUnitNative.Day:
					return new TimeSpan(1, 0, 0, 0, 0);
				case DateTimeMeasureUnitNative.Hour:
					return new TimeSpan(0, 1, 0, 0, 0);
				case DateTimeMeasureUnitNative.Millisecond:
					return new TimeSpan(0, 0, 0, 0, 1);
				case DateTimeMeasureUnitNative.Minute:
					return new TimeSpan(0, 0, 1, 0, 0);
				case DateTimeMeasureUnitNative.Month:
					return new TimeSpan(30, 0, 0, 0, 0);
				case DateTimeMeasureUnitNative.Quarter:
					return new TimeSpan(90, 0, 0, 0, 0);
				case DateTimeMeasureUnitNative.Second:
					return new TimeSpan(0, 0, 0, 1, 0);
				case DateTimeMeasureUnitNative.Week:
					return new TimeSpan(7, 0, 0, 0, 0);
				case DateTimeMeasureUnitNative.Year:
					return new TimeSpan(365, 0, 0, 0, 0);
				default:
					throw new Exception("Unknown DateTimeMeasureUnit.");
			}
		}
		bool IsDateGreaterThanMeasureUnit(DateTime date, TimeSpan measureUnitTimeSpan) {
			return (date - DateTime.MinValue).TotalMilliseconds > measureUnitTimeSpan.TotalMilliseconds;
		}
		protected override RangeSnapshot UpdateVisualRange(MinMaxValues visualRangeValues) {
			if (!visualRangeValues.HasValues) {
				return DefaultRange(VisualRange);
			}
			else {
				InternalRange corectedInternalValues;
				if (NeedCorrect(VisualRange))
					corectedInternalValues = CorrectWithSideMargins(VisualRange, visualRangeValues);
				else
					corectedInternalValues = CheckInternalValue(VisualRange, visualRangeValues);
				NativeRange nativeValues = CorrectNativeValues(visualRangeValues, corectedInternalValues.Values, VisualRange);
				return new RangeSnapshot(nativeValues, corectedInternalValues);
			}
		}
		protected override InternalRange CheckInternalValue(IAxisRangeData range, MinMaxValues internalValues) {
			InternalRange corectedInternalValues = new InternalRange();
			corectedInternalValues.Values = internalValues;
			if (double.IsNaN(corectedInternalValues.Values.Min))
				corectedInternalValues.Values.Min = corectedInternalValues.Values.Max - 1;
			if (double.IsNaN(corectedInternalValues.Values.Max))
				corectedInternalValues.Values.Max = corectedInternalValues.Values.Min + 1;
			if (corectedInternalValues.Values.Min == corectedInternalValues.Values.Max)
				corectedInternalValues.Values.Max = corectedInternalValues.Values.Min + 1;
			corectedInternalValues.Values.Min -= VisualRange.SideMarginsMin;
			corectedInternalValues.Values.Min += VisualRange.SideMarginsMax;
			corectedInternalValues.SetSideMargins(VisualRange);
			return corectedInternalValues;
		}
		protected override InternalRange CorrectInternalValue(MinMaxValues internalValues, double sideMargin, bool autoCorrectionMin, bool autoCorrectionMax) {
			InternalRange corectedInternalValues = base.CorrectInternalValue(internalValues, sideMargin, autoCorrectionMin, autoCorrectionMax);
			if (corectedInternalValues.Values.Min < 0) {
				corectedInternalValues.SideMargins.Min = corectedInternalValues.SideMargins.Min + corectedInternalValues.Values.Min;
				corectedInternalValues.Values.Min = 0;
			}
			if (corectedInternalValues.Values.Min == corectedInternalValues.Values.Max) {
				corectedInternalValues.Values = new MinMaxValues(internalValues.Min - defaultSideMarginValue, internalValues.Max + defaultSideMarginValue);
				corectedInternalValues.SideMargins = new MinMaxValues(defaultSideMarginValue, defaultSideMarginValue);
			}
			return corectedInternalValues;
		}
		protected override NativeRange CorrectNativeValues(MinMaxValues internalValues, MinMaxValues corectedInternalValues, IAxisRangeData range) {
			if (internalValues.Min < 0) {
				internalValues.Max += -internalValues.Min;
				internalValues.Min = 0;
			}
			if (internalValues.Min > internalValues.Max)
				return InternalToNative(corectedInternalValues);
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
			if (internalValues.Min == internalValues.Max) {
				if (VisualRange.MinValue != null && VisualRange.MaxValue != null && range.CorrectionMode != RangeCorrectionMode.Auto) {
					double refDif = Map.NativeToRefined(VisualRange.MaxValue) - Map.NativeToRefined(VisualRange.MinValue);
					DateTime dateMin = (DateTime)Map.InternalToNative(internalValues.Min);
					DateTime dateMax = (DateTime)Map.InternalToNative(internalValues.Min + Map.RefinedToInternal(refDif));
					return new NativeRange(dateMin, dateMax);
				}
				else {
					TimeSpan measureUnitTimeSpan = GetMeasureUnitTimeSpan();
					DateTime date = (DateTime)Map.InternalToNative(internalValues.Min);
					if(IsDateGreaterThanMeasureUnit(date, measureUnitTimeSpan))
						return new NativeRange(date - measureUnitTimeSpan, date + measureUnitTimeSpan);
					else
						return new NativeRange(date, date + measureUnitTimeSpan);
				}
			}
			return InternalToNative(internalValues);
		}
		protected override RangeSnapshot DefaultRange(IAxisRangeData range) {
			TimeSpan measureUnitTimeSpan = GetMeasureUnitTimeSpan();
			InternalRange internalRange = new InternalRange();
			internalRange.Values = new MinMaxValues(Map.NativeToInternal(DateTime.Today), Map.NativeToInternal(DateTime.Today + measureUnitTimeSpan));
			NativeRange nativeRange = CorrectNativeValues(internalRange.Values, internalRange.Values, range);
			internalRange.Values = new MinMaxValues(Map.NativeToInternal(nativeRange.Min), Map.NativeToInternal(nativeRange.Max));
			internalRange.SideMargins = new MinMaxValues(0, 0);
			internalRange.SideMarginsValue = range.SideMarginsValue;
			RangeSnapshot rangeSnapshot = new RangeSnapshot(nativeRange, internalRange);
			return rangeSnapshot;
		}
	}
}
