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
	#region Mass
	public enum Mass {
		Gram,
		Slug, 
		Pound, 
		AtomicMassUnit, 
		Ounce, 
		Grain,
		Hundredweight, 
		HundredweightImperial, 
		Stone,
		Ton,
		TonImperial, 
		Lbm,
		Ozm,
		Cwt,
	}
	#endregion
	#region MassUnitsConverter
	public class MassUnitsConverter : BaseUnitsConverter<Mass> {
		static readonly Dictionary<Mass, string> unitNameTable = CreateUnitNameTable();
		static Dictionary<Mass, string> CreateUnitNameTable() {
			Dictionary<Mass, string> result = new Dictionary<Mass, string>();
			result.Add(Mass.Gram, "g");
			result.Add(Mass.Slug, "sg");
			result.Add(Mass.Pound, "lbm");
			result.Add(Mass.Lbm, "lbm");
			result.Add(Mass.AtomicMassUnit, "u");
			result.Add(Mass.Ounce, "ozm");
			result.Add(Mass.Ozm, "ozm");
			result.Add(Mass.Grain, "grain");
			result.Add(Mass.Hundredweight, "cwt");
			result.Add(Mass.Cwt, "cwt");
			result.Add(Mass.HundredweightImperial, "lcwt");
			result.Add(Mass.Stone, "stone");
			result.Add(Mass.Ton, "ton");
			result.Add(Mass.TonImperial, "brton");
			return result;
		}
		protected override UnitCategory Category { get { return FunctionConvert.Mass; } }
		protected override Dictionary<Mass, string> UnitNameTable { get { return unitNameTable; } }
	}
	#endregion
	#region MassExtensions
	public static class MassExtensions {
		static readonly UnitCategory category = FunctionConvert.Mass;
		static UnitCategory Category { get { return category; } }
		static readonly int gramIndex = Category.UnitIndices["g"];
		static readonly int slugIndex = Category.UnitIndices["sg"];
		static readonly int poundIndex = Category.UnitIndices["lbm"];
		static readonly int atomicMassUnitIndex = Category.UnitIndices["u"];
		static readonly int ounceIndex = Category.UnitIndices["ozm"];
		static readonly int grainIndex = Category.UnitIndices["grain"];
		static readonly int hundredweightIndex = Category.UnitIndices["cwt"];
		static readonly int hundredweightImperialIndex = Category.UnitIndices["lcwt"];
		static readonly int stoneIndex = Category.UnitIndices["stone"];
		static readonly int tonIndex = Category.UnitIndices["ton"];
		static readonly int tonImperialIndex = Category.UnitIndices["brton"];
		const int kilogramIndex = -1;
		static QuantityValue<Mass> CreateValue(double value, int index) {
			QuantityValue<Mass> result = new QuantityValue<Mass>();
			result.Value = value;
			result.Category = Category;
			result.Index = index;
			return result;
		}
		static QuantityValue<Mass> ConvertValue(QuantityValue<Mass> value, int index) {
			if (value.Index < 0) { 
				value.Index = 0;
				value.Value *= 1000.0;
			}
			double convertedValue = Category.Convert(value.Value, value.Index, (index < 0 ? 0 : index));
			if (index < 0) { 
				convertedValue /= 1000.0;
			}
			return CreateValue(convertedValue, index);
		}
		#region Creation (double)
		public static QuantityValue<Mass> Kilograms(this double value) { return CreateValue(value, kilogramIndex); }
		public static QuantityValue<Mass> Grams(this double value) { return CreateValue(value, gramIndex); }
		public static QuantityValue<Mass> Slugs(this double value) { return CreateValue(value, slugIndex); }
		public static QuantityValue<Mass> Pounds(this double value) { return CreateValue(value, poundIndex); }
		public static QuantityValue<Mass> AtomicMassUnits(this double value) { return CreateValue(value, atomicMassUnitIndex); }
		public static QuantityValue<Mass> OunceMass(this double value) { return CreateValue(value, ounceIndex); }
		public static QuantityValue<Mass> Grains(this double value) { return CreateValue(value, grainIndex); }
		public static QuantityValue<Mass> Hundredweights(this double value) { return CreateValue(value, hundredweightIndex); }
		public static QuantityValue<Mass> HundredweightsImperial(this double value) { return CreateValue(value, hundredweightImperialIndex); }
		public static QuantityValue<Mass> Stones(this double value) { return CreateValue(value, stoneIndex); }
		public static QuantityValue<Mass> Tons(this double value) { return CreateValue(value, tonIndex); }
		public static QuantityValue<Mass> TonsImperial(this double value) { return CreateValue(value, tonImperialIndex); }
		#endregion
		#region Creation (float)
		public static QuantityValue<Mass> Kilograms(this float value) { return CreateValue(value, kilogramIndex); }
		public static QuantityValue<Mass> Grams(this float value) { return CreateValue(value, gramIndex); }
		public static QuantityValue<Mass> Slugs(this float value) { return CreateValue(value, slugIndex); }
		public static QuantityValue<Mass> Pounds(this float value) { return CreateValue(value, poundIndex); }
		public static QuantityValue<Mass> AtomicMassUnits(this float value) { return CreateValue(value, atomicMassUnitIndex); }
		public static QuantityValue<Mass> OunceMass(this float value) { return CreateValue(value, ounceIndex); }
		public static QuantityValue<Mass> Grains(this float value) { return CreateValue(value, grainIndex); }
		public static QuantityValue<Mass> Hundredweights(this float value) { return CreateValue(value, hundredweightIndex); }
		public static QuantityValue<Mass> HundredweightsImperial(this float value) { return CreateValue(value, hundredweightImperialIndex); }
		public static QuantityValue<Mass> Stones(this float value) { return CreateValue(value, stoneIndex); }
		public static QuantityValue<Mass> Tons(this float value) { return CreateValue(value, tonIndex); }
		public static QuantityValue<Mass> TonsImperial(this float value) { return CreateValue(value, tonImperialIndex); }
		#endregion
		#region Creation (int)
		public static QuantityValue<Mass> Kilograms(this int value) { return CreateValue(value, kilogramIndex); }
		public static QuantityValue<Mass> Grams(this int value) { return CreateValue(value, gramIndex); }
		public static QuantityValue<Mass> Slugs(this int value) { return CreateValue(value, slugIndex); }
		public static QuantityValue<Mass> Pounds(this int value) { return CreateValue(value, poundIndex); }
		public static QuantityValue<Mass> AtomicMassUnits(this int value) { return CreateValue(value, atomicMassUnitIndex); }
		public static QuantityValue<Mass> OunceMass(this int value) { return CreateValue(value, ounceIndex); }
		public static QuantityValue<Mass> Grains(this int value) { return CreateValue(value, grainIndex); }
		public static QuantityValue<Mass> Hundredweights(this int value) { return CreateValue(value, hundredweightIndex); }
		public static QuantityValue<Mass> HundredweightsImperial(this int value) { return CreateValue(value, hundredweightImperialIndex); }
		public static QuantityValue<Mass> Stones(this int value) { return CreateValue(value, stoneIndex); }
		public static QuantityValue<Mass> Tons(this int value) { return CreateValue(value, tonIndex); }
		public static QuantityValue<Mass> TonsImperial(this int value) { return CreateValue(value, tonImperialIndex); }
		#endregion
		#region Conversion
		public static QuantityValue<Mass> ToKilograms(this QuantityValue<Mass> value) { return ConvertValue(value, kilogramIndex); }
		public static QuantityValue<Mass> ToGrams(this QuantityValue<Mass> value) { return ConvertValue(value, gramIndex); }
		public static QuantityValue<Mass> ToSlugs(this QuantityValue<Mass> value) { return ConvertValue(value, slugIndex); }
		public static QuantityValue<Mass> ToPounds(this QuantityValue<Mass> value) { return ConvertValue(value, poundIndex); }
		public static QuantityValue<Mass> ToAtomicMassUnits(this QuantityValue<Mass> value) { return ConvertValue(value, atomicMassUnitIndex); }
		public static QuantityValue<Mass> ToOunceMass(this QuantityValue<Mass> value) { return ConvertValue(value, ounceIndex); }
		public static QuantityValue<Mass> ToGrains(this QuantityValue<Mass> value) { return ConvertValue(value, grainIndex); }
		public static QuantityValue<Mass> ToHundredweights(this QuantityValue<Mass> value) { return ConvertValue(value, hundredweightIndex); }
		public static QuantityValue<Mass> ToHundredweightsImperial(this QuantityValue<Mass> value) { return ConvertValue(value, hundredweightImperialIndex); }
		public static QuantityValue<Mass> ToStones(this QuantityValue<Mass> value) { return ConvertValue(value, stoneIndex); }
		public static QuantityValue<Mass> ToTons(this QuantityValue<Mass> value) { return ConvertValue(value, tonIndex); }
		public static QuantityValue<Mass> ToTonsImperial(this QuantityValue<Mass> value) { return ConvertValue(value, tonImperialIndex); }
		#endregion
	}
	#endregion
}
