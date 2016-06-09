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
	public class RangeHelper {
		public static double defaultSideMarginValue = 0.5;
		public static bool RangeDataSwitch = true;		
		public static bool RangeCalculationSwitch = true;
		static void SetSideMargins(IAxisRangeData range, MinMaxValues sideMargin) {
			range.SideMarginsMin = sideMargin.Min;
			range.SideMarginsMax = sideMargin.Max;
		}
		public static void SetDefaultRange(IAxisData axis) {
			if (axis == null || axis.FixedRange)
				return;
			IWholeAxisRangeData wholeRange = axis != null ? axis.WholeRange : null;
			IVisualAxisRangeData visualRange = axis != null ? axis.VisualRange : null;
			if (wholeRange != null)
				wholeRange.UpdateRange(0, 1, 0, 1);
			if (visualRange != null && wholeRange != null)
				SynchronizeVisualRange(wholeRange, visualRange, true);
		}
		public static void ResetRangeOnRemoveSeriesFromSeriesGroups(IAxisData axisData) {
			if (axisData != null && axisData.WholeRange != null) {
				if (axisData.WholeRange.CorrectionMode == RangeCorrectionMode.Auto && axisData.VisualRange.CorrectionMode == RangeCorrectionMode.Auto) {
					RangeHelper.SetDefaultRange(axisData);
				}
			}
		}
		public static void ThrowRangeData(IAxisData axisData, IAxisRangeData pattern, IAxisRange target) {
			double min = pattern.Min;
			double max = pattern.Max;
			if (axisData.AxisScaleTypeMap.ScaleType == ActualScaleType.DateTime) {
				AxisDateTimeMap priorScaleMap = axisData.AxisScaleTypeMap as AxisDateTimeMap;
				min = (double)((decimal)pattern.Min - (decimal)priorScaleMap.Min);
				max = (double)((decimal)pattern.Max - (decimal)priorScaleMap.Min);
			}
			else {
				min = axisData.AxisScaleTypeMap.Transformation.TransformForward(min);
				max = axisData.AxisScaleTypeMap.Transformation.TransformForward(max);
			}
			target.Assign(pattern);
			target.UpdateRange(pattern.MinValue, pattern.MaxValue, min, max);
		}
		public static void SynchronizeWholeRange(IWholeAxisRangeData wholeRange, IVisualAxisRangeData visualRange) {
			if (visualRange.Max > wholeRange.Max)
				SetMax(visualRange.MaxValue, visualRange.Max, wholeRange);
			if (visualRange.Min < wholeRange.Min)
				SetMin(visualRange.MinValue, visualRange.Min, wholeRange);
		}
		public static void SynchronizeVisualRange(IWholeAxisRangeData wholeRange, IVisualAxisRangeData visualRange, bool needSyncWhisWholeRange) {
			if (needSyncWhisWholeRange) {
				visualRange.UpdateRange(wholeRange.MinValue, wholeRange.MaxValue, wholeRange.Min, wholeRange.Max);
				SetSideMargins(visualRange, new MinMaxValues(wholeRange.SideMarginsMin, wholeRange.SideMarginsMax));
			}
			else {
				if (visualRange.Max > wholeRange.Max) {
					SetMax(wholeRange.MaxValue, wholeRange.Max, visualRange);
					visualRange.SideMarginsMax = wholeRange.SideMarginsMax;
				}
				if (visualRange.Min < wholeRange.Min) {
					SetMin(wholeRange.MinValue, wholeRange.Min, visualRange);
					visualRange.SideMarginsMin = wholeRange.SideMarginsMin;
				}
			}
		}
		static void SetMin(object minValue, double minValueInternal, IAxisRangeData range) {
			range.UpdateRange(minValue, range.MaxValue, minValueInternal, range.Max);
		}
		static void SetMax(object maxValue, double maxValueInternal, IAxisRangeData range) {
			range.UpdateRange(range.MinValue, maxValue, range.Min, maxValueInternal);
		}
	}
}
