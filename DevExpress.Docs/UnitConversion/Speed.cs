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
	#region Speed
	public enum Speed {
		Knot,
		KnotAdmiralty,
		MetersPerHour,
		MetersPerSecond,
		MilesPerHour,
		Mph
	}
	#endregion
	#region SpeedUnitsConverter
	public class SpeedUnitsConverter : BaseUnitsConverter<Speed> {
		static readonly Dictionary<Speed, string> unitNameTable = CreateUnitNameTable();
		static Dictionary<Speed, string> CreateUnitNameTable() {
			Dictionary<Speed, string> result = new Dictionary<Speed, string>();
			result.Add(Speed.Knot, "kn");
			result.Add(Speed.KnotAdmiralty, "admkn");
			result.Add(Speed.MetersPerHour, "m/h");
			result.Add(Speed.MetersPerSecond, "m/s");
			result.Add(Speed.MilesPerHour, "mph");
			result.Add(Speed.Mph, "mph");
			return result;
		}
		protected override UnitCategory Category { get { return FunctionConvert.Speed; } }
		protected override Dictionary<Speed, string> UnitNameTable { get { return unitNameTable; } }
	}
	#endregion
	#region SpeedExtensions
	public static class SpeedExtensions {
		static readonly UnitCategory category = FunctionConvert.Speed;
		static UnitCategory Category { get { return category; } }
		static readonly int knotIndex = Category.UnitIndices["kn"];
		static readonly int admiraltyKnotIndex = Category.UnitIndices["admkn"];
		static readonly int metersPerHourIndex = Category.UnitIndices["m/h"];
		static readonly int metersPerSecondIndex = Category.UnitIndices["m/s"];
		static readonly int milesPerHourIndex = Category.UnitIndices["mph"];
		static QuantityValue<Speed> CreateValue(double value, int index) {
			QuantityValue<Speed> result = new QuantityValue<Speed>();
			result.Value = value;
			result.Category = Category;
			result.Index = index;
			return result;
		}
		static QuantityValue<Speed> ConvertValue(QuantityValue<Speed> value, int index) {
			return CreateValue(Category.Convert(value.Value, value.Index, index), index);
		}
		#region Creation (double)
		public static QuantityValue<Speed> Knots(this double value) { return CreateValue(value, knotIndex); }
		public static QuantityValue<Speed> KnotsAdmiralty(this double value) { return CreateValue(value, admiraltyKnotIndex); }
		public static QuantityValue<Speed> MetersPerHour(this double value) { return CreateValue(value, metersPerHourIndex); }
		public static QuantityValue<Speed> MetersPerSecond(this double value) { return CreateValue(value, metersPerSecondIndex); }
		public static QuantityValue<Speed> MilesPerHour(this double value) { return CreateValue(value, milesPerHourIndex); }
		#endregion
		#region Creation (float)
		public static QuantityValue<Speed> Knots(this float value) { return CreateValue(value, knotIndex); }
		public static QuantityValue<Speed> KnotsAdmiralty(this float value) { return CreateValue(value, admiraltyKnotIndex); }
		public static QuantityValue<Speed> MetersPerHour(this float value) { return CreateValue(value, metersPerHourIndex); }
		public static QuantityValue<Speed> MetersPerSecond(this float value) { return CreateValue(value, metersPerSecondIndex); }
		public static QuantityValue<Speed> MilesPerHour(this float value) { return CreateValue(value, milesPerHourIndex); }
		#endregion
		#region Creation (int)
		public static QuantityValue<Speed> Knots(this int value) { return CreateValue(value, knotIndex); }
		public static QuantityValue<Speed> KnotsAdmiralty(this int value) { return CreateValue(value, admiraltyKnotIndex); }
		public static QuantityValue<Speed> MetersPerHour(this int value) { return CreateValue(value, metersPerHourIndex); }
		public static QuantityValue<Speed> MetersPerSecond(this int value) { return CreateValue(value, metersPerSecondIndex); }
		public static QuantityValue<Speed> MilesPerHour(this int value) { return CreateValue(value, milesPerHourIndex); }
		#endregion
		#region Conversion
		public static QuantityValue<Speed> ToKnots(this QuantityValue<Speed> value) { return ConvertValue(value, knotIndex); }
		public static QuantityValue<Speed> ToKnotsAdmiralty(this QuantityValue<Speed> value) { return ConvertValue(value, admiraltyKnotIndex); }
		public static QuantityValue<Speed> ToMetersPerHour(this QuantityValue<Speed> value) { return ConvertValue(value, metersPerHourIndex); }
		public static QuantityValue<Speed> ToMetersPerSecond(this QuantityValue<Speed> value) { return ConvertValue(value, metersPerSecondIndex); }
		public static QuantityValue<Speed> ToMilesPerHour(this QuantityValue<Speed> value) { return ConvertValue(value, milesPerHourIndex); }
		#endregion
	}
	#endregion
}
