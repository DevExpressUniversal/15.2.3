#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections;
using DevExpress.DataAccess.Native.Data;
namespace DevExpress.DashboardCommon.Native {
	public static class FormatConditionBarCalculator {
		public static decimal? CalcNormalizedValue(IMinMaxInfo minMaxInfo, IFormatCondition condition, IFormatConditionValueProvider valueProvider, bool allowNegativeAxis) {
			object value = ValueManager.ConvertToNumber(valueProvider.GetValue(condition));
			if(value == null)
				return null;
			decimal number = Convert.ToDecimal(value);
			decimal min, max;
			CalculateMinMaxValue(out min, out max, minMaxInfo);
			decimal range = CalculateRangeCore(min, max);
			number = GetNearToRangeNumber(number, min, max);
			if(range == decimal.Zero)
				range = decimal.One;
			decimal zeroPoint = allowNegativeAxis ? 0 : min;
			return (number - zeroPoint) / range;
		}
		static decimal GetNearToRangeNumber(decimal value, decimal min, decimal max) {
			if(value < min)
				value = min;
			if(value > max)
				value = max;
			return value;
		}
		public static decimal CalcZeroPosition(IMinMaxInfo minMaxInfo, bool allowNegativeAxis) {
			if(allowNegativeAxis) {
				decimal zeroPosition;
				decimal min, max;
				CalculateMinMaxValue(out min, out max, minMaxInfo);
				decimal minAbs = Math.Abs(min);
				if(min < 0)
					if(max < 0)
						zeroPosition = decimal.One;
					else {
						decimal range = CalculateRangeCore(min, max);
						zeroPosition = range != decimal.Zero ? minAbs / range : decimal.Zero;
					}
				else
					zeroPosition = decimal.Zero;
				return zeroPosition;
			}
			return decimal.Zero;
		}
		static void CalculateMinMaxValue(out decimal minimum, out decimal maximum, IMinMaxInfo minMaxInfo) {
			maximum = minimum = 0;
			decimal min = (minMaxInfo.MinimumType == DashboardFormatConditionValueType.Number) ? minMaxInfo.Minimum : minMaxInfo.AbsoluteMinimum;
			decimal max = (minMaxInfo.MaximumType == DashboardFormatConditionValueType.Number) ? minMaxInfo.Maximum : minMaxInfo.AbsoluteMaximum;
			bool equalTypes = minMaxInfo.MinimumType == minMaxInfo.MaximumType;
			if(minMaxInfo.MinimumType == DashboardFormatConditionValueType.Percent)
				min = Convert.ToDecimal(ValueManager.CalculatePercentCore(minMaxInfo.AbsoluteMinimum, minMaxInfo.AbsoluteMaximum, minMaxInfo.Minimum));
			if(minMaxInfo.MaximumType == DashboardFormatConditionValueType.Percent)
				max = Convert.ToDecimal(ValueManager.CalculatePercentCore(minMaxInfo.AbsoluteMinimum, minMaxInfo.AbsoluteMaximum, minMaxInfo.Maximum));
			minimum = min < max ? min : max;
			maximum = max > min ? max : min;
		}
		static decimal CalculateRangeCore(decimal min, decimal max) {
			decimal range;
			decimal minAbs = Math.Abs(min);
			decimal maxAbs = Math.Abs(max);
			if(min < 0)
				if(max < 0)
					range = minAbs;
				else
					range = minAbs + maxAbs;
			else
				range = maxAbs;
			return range;
		}
	}
}
