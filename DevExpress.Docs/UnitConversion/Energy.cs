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
	#region Energy
	public enum Energy {
		Joule,
		Erg,
		CalorieThermodynamic,
		CalorieIT,
		ElectronVolt,
		HorsePowerHour,
		WattHour,
		FootPound,
		BritishThermalUnit,
		Ev,
		HPh,
		Wh,
		Flb,
		Btu
	}
	#endregion
	#region EnergyUnitsConverter
	public class EnergyUnitsConverter : BaseUnitsConverter<Energy> {
		static readonly Dictionary<Energy, string> unitNameTable = CreateUnitNameTable();
		static Dictionary<Energy, string> CreateUnitNameTable() {
			Dictionary<Energy, string> result = new Dictionary<Energy, string>();
			result.Add(Energy.Joule, "J");
			result.Add(Energy.Erg, "e");
			result.Add(Energy.CalorieThermodynamic, "c");
			result.Add(Energy.CalorieIT, "cal");
			result.Add(Energy.ElectronVolt, "eV");
			result.Add(Energy.Ev, "eV");
			result.Add(Energy.HorsePowerHour, "HPh");
			result.Add(Energy.HPh, "HPh");
			result.Add(Energy.WattHour, "Wh");
			result.Add(Energy.Wh, "Wh");
			result.Add(Energy.FootPound, "flb");
			result.Add(Energy.Flb, "flb");
			result.Add(Energy.BritishThermalUnit, "btu");
			result.Add(Energy.Btu, "btu");
			return result;
		}
		protected override UnitCategory Category { get { return FunctionConvert.Energy; } }
		protected override Dictionary<Energy, string> UnitNameTable { get { return unitNameTable; } }
	}
	#endregion
	#region EnergyExtensions
	public static class EnergyExtensions {
		static readonly UnitCategory category = FunctionConvert.Energy;
		static UnitCategory Category { get { return category; } }
		static readonly int jouleIndex = Category.UnitIndices["J"];
		static readonly int ergIndex = Category.UnitIndices["e"];
		static readonly int calorieThermodynamicIndex = Category.UnitIndices["c"];
		static readonly int calorieInternational = Category.UnitIndices["cal"];
		static readonly int electronVoltIndex = Category.UnitIndices["eV"];
		static readonly int horsePowerHourIndex = Category.UnitIndices["HPh"];
		static readonly int wattHourIndex = Category.UnitIndices["Wh"];
		static readonly int footPoundIndex = Category.UnitIndices["flb"];
		static readonly int britishThermalUnitIndex = Category.UnitIndices["btu"];
		static QuantityValue<Energy> CreateValue(double value, int index) {
			QuantityValue<Energy> result = new QuantityValue<Energy>();
			result.Value = value;
			result.Category = Category;
			result.Index = index;
			return result;
		}
		static QuantityValue<Energy> ConvertValue(QuantityValue<Energy> value, int index) {
			return CreateValue(Category.Convert(value.Value, value.Index, index), index);
		}
		#region Creation (double)
		public static QuantityValue<Energy> Joules(this double value) { return CreateValue(value, jouleIndex); }
		public static QuantityValue<Energy> Ergs(this double value) { return CreateValue(value, ergIndex); }
		public static QuantityValue<Energy> CaloriesThermodynamic(this double value) { return CreateValue(value, calorieThermodynamicIndex); }
		public static QuantityValue<Energy> CaloriesInternational(this double value) { return CreateValue(value, calorieInternational); }
		public static QuantityValue<Energy> ElectronVolts(this double value) { return CreateValue(value, electronVoltIndex); }
		public static QuantityValue<Energy> HorsePowerHours(this double value) { return CreateValue(value, horsePowerHourIndex); }
		public static QuantityValue<Energy> WattHours(this double value) { return CreateValue(value, wattHourIndex); }
		public static QuantityValue<Energy> FootPounds(this double value) { return CreateValue(value, footPoundIndex); }
		public static QuantityValue<Energy> BritishThermalUnits(this double value) { return CreateValue(value, britishThermalUnitIndex); }
		#endregion
		#region Creation (float)
		public static QuantityValue<Energy> Joules(this float value) { return CreateValue(value, jouleIndex); }
		public static QuantityValue<Energy> Ergs(this float value) { return CreateValue(value, ergIndex); }
		public static QuantityValue<Energy> CaloriesThermodynamic(this float value) { return CreateValue(value, calorieThermodynamicIndex); }
		public static QuantityValue<Energy> CaloriesInternational(this float value) { return CreateValue(value, calorieInternational); }
		public static QuantityValue<Energy> ElectronVolts(this float value) { return CreateValue(value, electronVoltIndex); }
		public static QuantityValue<Energy> HorsePowerHours(this float value) { return CreateValue(value, horsePowerHourIndex); }
		public static QuantityValue<Energy> WattHours(this float value) { return CreateValue(value, wattHourIndex); }
		public static QuantityValue<Energy> FootPounds(this float value) { return CreateValue(value, footPoundIndex); }
		public static QuantityValue<Energy> BritishThermalUnits(this float value) { return CreateValue(value, britishThermalUnitIndex); }
		#endregion
		#region Creation (int)
		public static QuantityValue<Energy> Joules(this int value) { return CreateValue(value, jouleIndex); }
		public static QuantityValue<Energy> Ergs(this int value) { return CreateValue(value, ergIndex); }
		public static QuantityValue<Energy> CaloriesThermodynamic(this int value) { return CreateValue(value, calorieThermodynamicIndex); }
		public static QuantityValue<Energy> CaloriesInternational(this int value) { return CreateValue(value, calorieInternational); }
		public static QuantityValue<Energy> ElectronVolts(this int value) { return CreateValue(value, electronVoltIndex); }
		public static QuantityValue<Energy> HorsePowerHours(this int value) { return CreateValue(value, horsePowerHourIndex); }
		public static QuantityValue<Energy> WattHours(this int value) { return CreateValue(value, wattHourIndex); }
		public static QuantityValue<Energy> FootPounds(this int value) { return CreateValue(value, footPoundIndex); }
		public static QuantityValue<Energy> BritishThermalUnits(this int value) { return CreateValue(value, britishThermalUnitIndex); }
		#endregion
		#region Conversion
		public static QuantityValue<Energy> ToJoules(this QuantityValue<Energy> value) { return ConvertValue(value, jouleIndex); }
		public static QuantityValue<Energy> ToErgs(this QuantityValue<Energy> value) { return ConvertValue(value, ergIndex); }
		public static QuantityValue<Energy> ToCaloriesThermodynamic(this QuantityValue<Energy> value) { return ConvertValue(value, calorieThermodynamicIndex); }
		public static QuantityValue<Energy> ToCaloriesInternational(this QuantityValue<Energy> value) { return ConvertValue(value, calorieInternational); }
		public static QuantityValue<Energy> ToElectronVolts(this QuantityValue<Energy> value) { return ConvertValue(value, electronVoltIndex); }
		public static QuantityValue<Energy> ToHorsePowerHours(this QuantityValue<Energy> value) { return ConvertValue(value, horsePowerHourIndex); }
		public static QuantityValue<Energy> ToWattHours(this QuantityValue<Energy> value) { return ConvertValue(value, wattHourIndex); }
		public static QuantityValue<Energy> ToFootPounds(this QuantityValue<Energy> value) { return ConvertValue(value, footPoundIndex); }
		public static QuantityValue<Energy> ToBritishThermalUnits(this QuantityValue<Energy> value) { return ConvertValue(value, britishThermalUnitIndex); }
		#endregion
	}
	#endregion
}
