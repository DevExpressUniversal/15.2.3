#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.UnitConversion {
	#region Time
	public enum Time {
		Year,
		Day,
		Hour,
		Minute,
		Second
	}
	#endregion
	#region TimeUnitsConverter
	public class TimeUnitsConverter : BaseUnitsConverter<Time> {
		static readonly Dictionary<Time, string> unitNameTable = CreateUnitNameTable();
		static Dictionary<Time, string> CreateUnitNameTable() {
			Dictionary<Time, string> result = new Dictionary<Time, string>();
			result.Add(Time.Year, "yr");
			result.Add(Time.Day, "day");
			result.Add(Time.Hour, "hr");
			result.Add(Time.Minute, "min");
			result.Add(Time.Second, "sec");
			return result;
		}
		protected override UnitCategory Category { get { return FunctionConvert.Time; } }
		protected override Dictionary<Time, string> UnitNameTable { get { return unitNameTable; } }
	}
	#endregion
	#region TimeExtensions
	public static class TimeExtensions {
		static readonly UnitCategory category = FunctionConvert.Time;
		static UnitCategory Category { get { return category; } }
		static readonly int yearIndex = Category.UnitIndices["yr"];
		static readonly int dayIndex = Category.UnitIndices["d"];
		static readonly int hourIndex = Category.UnitIndices["hr"];
		static readonly int minuteIndex = Category.UnitIndices["min"];
		static readonly int secondIndex = Category.UnitIndices["s"];
		static QuantityValue<Time> CreateValue(double value, int index) {
			QuantityValue<Time> result = new QuantityValue<Time>();
			result.Value = value;
			result.Category = Category;
			result.Index = index;
			return result;
		}
		static QuantityValue<Time> ConvertValue(QuantityValue<Time> value, int index) {
			return CreateValue(Category.Convert(value.Value, value.Index, index), index);
		}
		#region Creation (double)
		public static QuantityValue<Time> Years(this double value) { return CreateValue(value, yearIndex); }
		public static QuantityValue<Time> Days(this double value) { return CreateValue(value, dayIndex); }
		public static QuantityValue<Time> Hours(this double value) { return CreateValue(value, hourIndex); }
		public static QuantityValue<Time> Minutes(this double value) { return CreateValue(value, minuteIndex); }
		public static QuantityValue<Time> Seconds(this double value) { return CreateValue(value, secondIndex); }
		#endregion
		#region Creation (float)
		public static QuantityValue<Time> Years(this float value) { return CreateValue(value, yearIndex); }
		public static QuantityValue<Time> Days(this float value) { return CreateValue(value, dayIndex); }
		public static QuantityValue<Time> Hours(this float value) { return CreateValue(value, hourIndex); }
		public static QuantityValue<Time> Minutes(this float value) { return CreateValue(value, minuteIndex); }
		public static QuantityValue<Time> Seconds(this float value) { return CreateValue(value, secondIndex); }
		#endregion
		#region Creation (int)
		public static QuantityValue<Time> Years(this int value) { return CreateValue(value, yearIndex); }
		public static QuantityValue<Time> Days(this int value) { return CreateValue(value, dayIndex); }
		public static QuantityValue<Time> Hours(this int value) { return CreateValue(value, hourIndex); }
		public static QuantityValue<Time> Minutes(this int value) { return CreateValue(value, minuteIndex); }
		public static QuantityValue<Time> Seconds(this int value) { return CreateValue(value, secondIndex); }
		#endregion
		#region Conversion
		public static QuantityValue<Time> ToYears(this QuantityValue<Time> value) { return ConvertValue(value, yearIndex); }
		public static QuantityValue<Time> ToDays(this QuantityValue<Time> value) { return ConvertValue(value, dayIndex); }
		public static QuantityValue<Time> ToHours(this QuantityValue<Time> value) { return ConvertValue(value, hourIndex); }
		public static QuantityValue<Time> ToMinutes(this QuantityValue<Time> value) { return ConvertValue(value, minuteIndex); }
		public static QuantityValue<Time> ToSeconds(this QuantityValue<Time> value) { return ConvertValue(value, secondIndex); }
		#endregion
	}
	#endregion
}
