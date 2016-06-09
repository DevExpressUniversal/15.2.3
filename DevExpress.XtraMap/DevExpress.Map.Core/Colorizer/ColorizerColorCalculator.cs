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

using DevExpress.Compatibility.System.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.Map.Native {
	public static class MeasureRulesHelper {
		static SizeProportionsCalculator calculator = new SizeProportionsCalculator();
		public static double CalculateValue(IList<double> sizes, IList<double> ranges, bool approx, IRangeDistribution distribution, double value) {
			return calculator.CalculateProportionalValue(sizes, ranges, approx, distribution, value);
		}
	}
	public static class ColorizerColorHelper {
		static ColorProportionsCalculator calculator = new ColorProportionsCalculator();
		public static Color CalculateValue(IList<Color> colors, IList<double> ranges, bool approx, IRangeDistribution distribution, double value) {
			return calculator.CalculateProportionalValue(colors, ranges, approx, distribution, value);
		}
	}
	public abstract class ProportionsCalculator<T> {
		protected abstract T DefaultValue { get; }
		protected abstract bool SupportsMinMaxValues { get; }
		protected abstract T TransformValue(T fromUnit, T toUnit, double ratio);
		public T CalculateProportionalValue(IList<T> units, IList<double> ranges, bool approx, IRangeDistribution distribution, double value) {
			return approx ? GetApproximatedValue(units, ranges, distribution, value) : GetValue(units, ranges, value);
		}
		T GetApproximatedValue(IList<T> units, IList<double> ranges, IRangeDistribution distribution, double value) {
			double convertedValue = CalculateNormalizedValue(ranges, distribution, value);
			return CalculateApproximatedValue(units, convertedValue);
		}
		double CalculateNormalizedValue(IList<double> ranges, IRangeDistribution distribution, double value) {
			if(ranges.Count == 0)
				return 0.0;
			if (ranges.Count == 1)
				return CalculateSingleRangeStopNormalizedValue(ranges[0], value);
			int idx = GetRangeIndex(value, ranges);
			if (idx == -1)
				return 0.0;
			double normalizedValue = (double)idx / (ranges.Count - 1);
			return normalizedValue != 1.0 ? distribution.ConvertRangeValue(ranges[idx], ranges[idx + 1], value) / (ranges.Count - 1) + normalizedValue : 1.0;
		}
		double CalculateSingleRangeStopNormalizedValue(double pivotValue, double actualValue) {
			if (actualValue < pivotValue)
				return 0.0;
			if (actualValue > pivotValue)
				return 1.0;
			return 0.5;
		}
		int GetRangeIndex(double value, IList<double> ranges) {
			int range;
			int stopsCount = ranges.Count;
			for(range = 0; range < stopsCount; range++) {
				if(value < ranges[range])
					break;
			}
			return range - 1;
		}
		T GetValue(IList<T> units, IList<double> ranges, double value) {
			int unitsCount = units.Count;
			int stopsCount = ranges.Count;
			if(stopsCount <= 1 || unitsCount <= 1)
				return CalculateApproximatedValue(units, 0.5);
			if (stopsCount == 2 && SupportsMinMaxValues) {
				double step = (ranges[1] - ranges[0]) / unitsCount;
				int colorIndex = (int)((value - ranges[0])/ step);
				colorIndex = Math.Max(0, Math.Min(colorIndex, unitsCount - 1));
				return units[colorIndex];
			}
			int range = GetRangeIndex(value, ranges);
			int maxRangeIndex = stopsCount - 1;
			double normalizedValue = range != -1 ? ((double)range) / ((double)maxRangeIndex) : 0.0;
			return CalculateApproximatedValue(units, normalizedValue);
		}
		T CalculateApproximatedValue(IList<T> units, double value) {
			if(IsCollectionEmpty(units) || IsValueEmpty(value))
				return DefaultValue;
			int count = units.Count;
			int upperIndex = count - 1;
			if(count == 1)
				return units[0];
			double normalizedValue = LimitValue(value, 0.0, 1.0);
			double sizeRange = 1.0 / upperIndex;
			int sizeRangeIndex = (int)(normalizedValue / sizeRange);
			if(sizeRangeIndex >= upperIndex)
				return units[upperIndex];
			double ratio = (normalizedValue - (sizeRangeIndex * sizeRange)) / sizeRange;
			T fromUnit = units[sizeRangeIndex];
			T toUnit = units[sizeRangeIndex + 1];
			return sizeRangeIndex < (count - 1) ? TransformValue(fromUnit, toUnit, ratio) : units[count - 1];
		}
		double LimitValue(double value, double minLimit, double maxLimit) {
			return Math.Max(Math.Min(value, maxLimit), minLimit);
		}
		bool IsValueEmpty(double value) {
			return double.IsNaN(value) || double.IsInfinity(value);
		}
		bool IsCollectionEmpty(IList<T> collection) {
			return collection == null || collection.Count == 0;
		}
	}
	public class SizeProportionsCalculator : ProportionsCalculator<double> {
		const double DefaultSize = 0;
		protected override double DefaultValue {
			get { return DefaultSize; }
		}
		protected override bool SupportsMinMaxValues {
			get { return false; }
		}
		protected override double TransformValue(double fromUnit, double toUnit, double ratio) {
			return ((1.0 - ratio) * fromUnit + ratio * toUnit);
		}
	}
	public class ColorProportionsCalculator : ProportionsCalculator<Color> {
		static readonly Color DefaultColor = Color.FromArgb(0, 0, 0, 0);
		protected override Color DefaultValue {
			get { return DefaultColor; }
		}
		protected override bool SupportsMinMaxValues {
			get { return true; }
		}
		protected override Color TransformValue(Color fromUnit, Color toUnit, double ratio) {
			return Color.FromArgb(ConvertColorValue(fromUnit.A, toUnit.A, ratio),
								  ConvertColorValue(fromUnit.R, toUnit.R, ratio),
								  ConvertColorValue(fromUnit.G, toUnit.G, ratio),
								  ConvertColorValue(fromUnit.B, toUnit.B, ratio));
		}
		byte ConvertColorValue(byte fromValue, byte toValue, double ratio) {
			return (byte)(fromValue * (1.0 - ratio) + toValue * ratio);
		}
	}
}
