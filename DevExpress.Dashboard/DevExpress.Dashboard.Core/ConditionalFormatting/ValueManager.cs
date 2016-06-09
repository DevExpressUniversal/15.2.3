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
using System.Drawing;
using DevExpress.DashboardCommon.Native;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon {
	static class ValueManager {
		public static int CompareValues(object val1, object val2, bool changeType) {
			if(changeType && val1 != null && val2 != null) {
				bool firstNegativeInfinity = object.Equals(val1, RangeInfo.NegativeInfinity);
				bool secondNegativeInfinity = object.Equals(val2, RangeInfo.NegativeInfinity);
				if(firstNegativeInfinity && secondNegativeInfinity)
					return 0;
				if(firstNegativeInfinity)
					return -1;
				if(secondNegativeInfinity)
					return 1;
				Type val1Type = val1.GetType();
				Type val2Type = val2.GetType();
				if(!val1Type.Equals(val2Type)) {
					try {
						val2 = Convert.ChangeType(val2, val1Type);
					}
					catch { }
				}
			}
			return Comparer.Default.Compare(val1, val2);
		}
		public static int CompareValues(object val1, object val2, DataFieldType type) {
			return CompareValues(Helper.ConvertToType(val1, type), Helper.ConvertToType(val2, type), false);
		}
		public static object ConvertToNumber(object value) {
			if(!IsNull(value)) {
				var tc = DXTypeExtensions.GetTypeCode(value.GetType());
				if(tc == TypeCode.DateTime)
					return ((DateTime)value).Ticks;
				if(tc >= TypeCode.SByte && tc <= TypeCode.Decimal)
					return value;
			}
			return null;
		}
		public static bool IsNull(object value) {
			return (value == null || object.ReferenceEquals(value, DBNull.Value));
		}
		public static object CalculatePercentCore(object min, object max, object percent) {
			if(ValueManager.CompareValues(min, max, true) >= 0)
				return max;
			if(ValueManager.CompareValues(percent, 0, true) <= 0)
				return min;
			if(ValueManager.CompareValues(percent, 100, true) >= 0)
				return max;
			decimal numericMin = Convert.ToDecimal(min);
			decimal numericMax = Convert.ToDecimal(max);
			decimal numericPercent = Convert.ToDecimal(percent);
			decimal delta = numericMax - numericMin;
			return numericMin + (delta * (numericPercent / 100));
		}
		public static object CalculatePercent(object min, object max, object percent) {
			object minValue = ConvertToNumber(min);
			object maxValue = ConvertToNumber(max);
			object percentValue = ConvertToNumber(percent);
			if(minValue != null && maxValue != null && percentValue != null)
				return CalculatePercentCore(minValue, maxValue, percentValue);
			return null;
		}
		public static decimal[] CalculateRangePercentValues(int segmentCount) {
			decimal[] values = new decimal[segmentCount];
			for(int i = 0; i < segmentCount; i++)
				values[i] = ValueManager.CalculateRangePercent(i, segmentCount);
			return values;
		}
		public static decimal CalculateRangePercent(int index, int count, int decimals = 0) {
			return Math.Round(100m * (index / (decimal)count), decimals);
		}
		public static Color CalculateColor(Color left, Color right, int index, int count) {
			return ValueMapScaleHelper.GetGradientColor(left, right, index, count); 
		}
	}
}
