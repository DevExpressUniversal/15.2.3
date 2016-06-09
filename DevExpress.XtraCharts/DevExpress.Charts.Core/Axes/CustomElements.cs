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
namespace DevExpress.Charts.Native {
	public interface IScaleBreak {
		IAxisValueContainer Edge1 { get; }
		IAxisValueContainer Edge2 { get; }
		bool Visible { get; }
	}
	public interface IStripLimit : IAxisValueContainer {
		double Value { get; }
	}
	public interface IStrip {
		bool Visible { get; }
		IStripLimit MinLimit { get; }
		IStripLimit MaxLimit { get; }
		string AxisLabelText { get; }
		void CorrectLimits();
	}
	public interface IConstantLine : IAxisValueContainer {
		bool Visible { get; }
	}
	public interface ICustomAxisLabel : IAxisValueContainer {
		bool Visible { get; }
		object Content { get; }
	}
	public static class CustomAxisElementsHelper {
		static bool IsAxisValueContainerVisible(IAxisValueContainer container) {
			return IsAxisValueVisible(container.Axis, container.GetAxisValue());
		}
		public static bool IsAxisValueVisible(IAxisData axis, object axisValue) {
			if (!axis.AxisScaleTypeMap.IsCompatible(axisValue))
				return false;
			return axis.AxisScaleTypeMap.ScaleType != ActualScaleType.DateTime || axis.DateTimeScaleOptions.WorkdaysOptions == null || !axis.DateTimeScaleOptions.WorkdaysOptions.WorkdaysOnly ||
				!DateTimeUtils.IsHoliday(axis.DateTimeScaleOptions.WorkdaysOptions, (DateTime)axisValue, true, true);
		}
		public static bool IsStripVisible(IScaleMap map, IStrip strip) {
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
		public static bool IsConstantLineVisible(IConstantLine constantLine) {
			return constantLine.Visible && IsAxisValueContainerVisible(constantLine);
		}
		public static bool IsCustomLabelVisible(ICustomAxisLabel label) {
			return label.Visible && IsAxisValueContainerVisible(label);
		}
	}
	public static class CustomAxisElementsHelperNew {
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
		public static bool IsAxisValueVisible(IAxisData axis, object axisValue) {
			if (!axis.AxisScaleTypeMap.IsCompatible(axisValue))
				return false;
			return axis.AxisScaleTypeMap.ScaleType != ActualScaleType.DateTime || axis.DateTimeScaleOptions.WorkdaysOptions == null || !axis.DateTimeScaleOptions.WorkdaysOptions.WorkdaysOnly ||
				!DateTimeUtils.IsHoliday(axis.DateTimeScaleOptions.WorkdaysOptions, (DateTime)axisValue, true, true);
		}
		public static bool IsStripVisible(IAxisGridMapping map, IStrip strip) {
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
		public static bool IsStripVisibleInGrid(IMinMaxValues limits, IAxisGridMapping map, IStrip strip) {
			return IsStripVisible(map, strip) && !String.IsNullOrEmpty(strip.AxisLabelText) &&
				!Double.IsNaN(CalcStripGridValue(strip, limits));
		}
		public static void FillCustomTextData(IAxisElementContainer axis, IAxisGridMapping map, IMinMaxValues limits, AxisGridDataEx gridData, AxisTextDataEx textData) {
			if (axis.Strips != null) {
				foreach (IStrip strip in axis.Strips)
					if (IsStripVisibleInGrid(limits, map, strip))
						textData.Add(gridData, null, CalcStripGridValue(strip, limits), strip.AxisLabelText, true);
			}
			if (axis.CustomLabels != null) {
				Transformation transformation = map.Transformation;
				foreach (ICustomAxisLabel label in axis.CustomLabels)
					if (label.Visible && IsAxisValueVisible((IAxisData)axis, label.GetAxisValue()))
						textData.Add(gridData, label, label.GetValue(), label.Content, true);
			}
		}
	}
}
