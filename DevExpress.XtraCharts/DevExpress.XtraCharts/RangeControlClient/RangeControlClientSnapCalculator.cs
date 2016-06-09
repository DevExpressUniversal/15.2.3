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
using System.Linq;
using System.Text;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class RangeControlGridUnit {
		readonly double step;
		readonly double spacing;
		readonly double offset;
		public double Step { get { return step; } }
		public double Spacing { get { return spacing; } }
		public double Offset { get { return offset; } }
		public double ReversedOffset { get { return spacing - offset; } }
		public bool IsValid { get { return !double.IsNaN(spacing) && !double.IsNaN(offset) && !double.IsNaN(step); } }
		public RangeControlGridUnit(double spacing, double step, double offset) {
			this.spacing = spacing;
			this.step = step;
			this.offset = offset;
		}
	}
	public class AlignedRangeControlGridUnit : RangeControlGridUnit {
		readonly DateTimeMeasureUnitNative alignment;
		public DateTimeMeasureUnitNative Alignment { get { return alignment; } }
		public AlignedRangeControlGridUnit(DateTimeMeasureUnitNative alignment, double spacing, double step, double offset)
			: base(spacing, step, offset) {
			this.alignment = alignment;
		}
	}
	public class RangeControlClientSnapCalculator {
		readonly AxisScaleTypeMap scaleMap;
		readonly RangeControlClientGridMapping gridMapping;
		readonly double axisRangeMin;
		readonly double axisRangeMax;
		readonly RangeControlMapping rangeControlMapping;
		double GetValue(double normalizedArgument) {
			double argumentDelta = axisRangeMax - axisRangeMin;
			return normalizedArgument * argumentDelta + axisRangeMin;
		}
		double NormalizeArgument(double argument) {
			double argumentDelta = axisRangeMax - axisRangeMin;
			double relativeArgument = argument - axisRangeMin;
			return relativeArgument / argumentDelta;
		}
		double Normalize(double value) {
			double normalValue = NormalizeArgument(value);
			if (normalValue < 0.0)
				normalValue = 0.0;
			else if (normalValue > 1.0)
				normalValue = 1.0;
			return normalValue;
		}
		double ShiftValue(RangeControlGridUnit unit, double value, double offset) {
			double floorIndex = gridMapping.FloorValue(unit, value, offset);
			return gridMapping.GetGridValue(unit, floorIndex);
		}
		double FloorValue(RangeControlGridUnit unit, double value) {
			double floorIndex = gridMapping.FloorValue(unit, value, 0.0);
			return gridMapping.GetGridValue(unit, floorIndex);
		}
		double CeilValue(RangeControlGridUnit unit, double value) {
			double ceilIndex = gridMapping.CeilValue(unit, value, 0.0);
			return gridMapping.GetGridValue(unit, ceilIndex);
		}
		int GetCorrectionDirectionBySideMargin(double value) {
			if (rangeControlMapping.ChartSideMargin <= double.Epsilon) {
				if (value < rangeControlMapping.DataRange.Min)
					return -1;
				if (value > rangeControlMapping.DataRange.Max)
					return 1;
			}
			return 0;
		}
		double CorrectValue(double value, int correction) {
			if (correction < 0)
				return rangeControlMapping.DataRange.Min;
			if (correction > 0)
				return rangeControlMapping.DataRange.Max;
			return value;
		}
		public double RoundValue(RangeControlGridUnit unit, double value, bool limitByRangeBounds) {
			double floor = gridMapping.GetGridValue(unit, gridMapping.FloorValue(unit, value, 0.0));
			double ceil = gridMapping.GetGridValue(unit, gridMapping.CeilValue(unit, value, 0.0));
			double alignedFloor = limitByRangeBounds ? Math.Max(axisRangeMin, floor) : floor;
			double alignedCeil = limitByRangeBounds ? Math.Min(axisRangeMax, ceil) : ceil;
			double delta = alignedCeil - alignedFloor;
			double offset = value - alignedFloor;
			return (offset / delta > 0.5) ? ceil : floor;
		}
		public MinMaxValues SnapRange(double rangeMin, double rangeMax, RangeValidationBase validationBase, RangeControlGridUnit unit) {
			int minCorrection = GetCorrectionDirectionBySideMargin(rangeMin);
			int maxCorrection = GetCorrectionDirectionBySideMargin(rangeMax);
			rangeMin = CorrectValue(rangeMin, minCorrection);
			rangeMax = CorrectValue(rangeMax, maxCorrection);
			double edgeMin = RoundValue(unit, rangeMin, true);
			double edgeMax = RoundValue(unit, rangeMax, true);
			if (edgeMin == edgeMax) {
				double edge = edgeMin;
				if (rangeMin == rangeMax) {
					switch (validationBase) {
						case RangeValidationBase.Average:
							if (edge > rangeMin || minCorrection > 0 || maxCorrection > 0)
								edgeMin = ShiftValue(unit, edge, -1);
							else
								edgeMax = ShiftValue(unit, edge, 1);
							break;
						case RangeValidationBase.Minimum:
							edgeMin = ShiftValue(unit, edge, -1);
							break;
						case RangeValidationBase.Maximum:
							edgeMax = ShiftValue(unit, edge, 1);
							break;
					}
				}
				else {
					double diffMin = Math.Abs(edge - rangeMin);
					double diffMax = Math.Abs(edge - rangeMax);
					if (diffMin > diffMax)
						edgeMin = FloorValue(unit, rangeMin);
					else
						edgeMax = CeilValue(unit, rangeMax);
				}
			}
			return new MinMaxValues(edgeMin, edgeMax);
		}
		public NormalizedRange SnapNormalRange(NormalizedRange range, RangeValidationBase validationBase, RangeControlGridUnit unit) {
			double min = GetValue(range.Minimum);
			double max = GetValue(range.Maximum);
			IMinMaxValues snapedRange = SnapRange(min, max, validationBase, unit);
			double normalMin = Normalize(snapedRange.Min);
			double normalMax = Normalize(snapedRange.Max);
			return new NormalizedRange(normalMin, normalMax);
		}
		public RangeControlClientSnapCalculator(ChartRangeControlClientGridOptions gridOptions, RangeControlMapping mapping) {
			this.scaleMap = gridOptions.Axis.ScaleTypeMap;
			this.gridMapping = gridOptions.ClientGridMapping;
			this.axisRangeMax = mapping.RangeControlRange.Max;
			this.axisRangeMin = mapping.RangeControlRange.Min;
			this.rangeControlMapping = mapping;
		}
	}
}
