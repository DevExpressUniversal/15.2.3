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
	#region Pressure
	public enum Pressure {
		Pascal,
		Atmosphere,
		MmHg,
		PoundPerSquareInch,
		Torr,
		P,
		Pa,
		Atm,
		Psi,
	}
	#endregion
	#region PressureUnitsConverter
	public class PressureUnitsConverter : BaseUnitsConverter<Pressure> {
		static readonly Dictionary<Pressure, string> unitNameTable = CreateUnitNameTable();
		static Dictionary<Pressure, string> CreateUnitNameTable() {
			Dictionary<Pressure, string> result = new Dictionary<Pressure, string>();
			result.Add(Pressure.Pascal, "Pa");
			result.Add(Pressure.P, "Pa");
			result.Add(Pressure.Pa, "Pa");
			result.Add(Pressure.Atmosphere, "atm");
			result.Add(Pressure.Atm, "atm");
			result.Add(Pressure.MmHg, "mmHg");
			result.Add(Pressure.PoundPerSquareInch, "psi");
			result.Add(Pressure.Psi, "psi");
			result.Add(Pressure.Torr, "Torr");
			return result;
		}
		protected override UnitCategory Category { get { return FunctionConvert.Pressure; } }
		protected override Dictionary<Pressure, string> UnitNameTable { get { return unitNameTable; } }
	}
	#endregion
	#region PressureExtensions
	public static class PressureExtensions {
		static readonly UnitCategory category = FunctionConvert.Pressure;
		static UnitCategory Category { get { return category; } }
		static readonly int pascalIndex = Category.UnitIndices["Pa"];
		static readonly int atmosphereIndex = Category.UnitIndices["atm"];
		static readonly int mmHgIndex = Category.UnitIndices["mmHg"];
		static readonly int psiIndex = Category.UnitIndices["psi"];
		static readonly int torrIndex = Category.UnitIndices["Torr"];
		static QuantityValue<Pressure> CreateValue(double value, int index) {
			QuantityValue<Pressure> result = new QuantityValue<Pressure>();
			result.Value = value;
			result.Category = Category;
			result.Index = index;
			return result;
		}
		static QuantityValue<Pressure> ConvertValue(QuantityValue<Pressure> value, int index) {
			return CreateValue(Category.Convert(value.Value, value.Index, index), index);
		}
		#region Creation (double)
		public static QuantityValue<Pressure> Pascals(this double value) { return CreateValue(value, pascalIndex); }
		public static QuantityValue<Pressure> Atmospheres(this double value) { return CreateValue(value, atmosphereIndex); }
		public static QuantityValue<Pressure> MmHg(this double value) { return CreateValue(value, mmHgIndex); }
		public static QuantityValue<Pressure> PoundsPerSquareInch(this double value) { return CreateValue(value, psiIndex); }
		public static QuantityValue<Pressure> Torrs(this double value) { return CreateValue(value, torrIndex); }
		#endregion
		#region Creation (float)
		public static QuantityValue<Pressure> Pascals(this float value) { return CreateValue(value, pascalIndex); }
		public static QuantityValue<Pressure> Atmospheres(this float value) { return CreateValue(value, atmosphereIndex); }
		public static QuantityValue<Pressure> MmHg(this float value) { return CreateValue(value, mmHgIndex); }
		public static QuantityValue<Pressure> PoundsPerSquareInch(this float value) { return CreateValue(value, psiIndex); }
		public static QuantityValue<Pressure> Torrs(this float value) { return CreateValue(value, torrIndex); }
		#endregion
		#region Creation (int)
		public static QuantityValue<Pressure> Pascals(this int value) { return CreateValue(value, pascalIndex); }
		public static QuantityValue<Pressure> Atmospheres(this int value) { return CreateValue(value, atmosphereIndex); }
		public static QuantityValue<Pressure> MmHg(this int value) { return CreateValue(value, mmHgIndex); }
		public static QuantityValue<Pressure> PoundsPerSquareInch(this int value) { return CreateValue(value, psiIndex); }
		public static QuantityValue<Pressure> Torrs(this int value) { return CreateValue(value, torrIndex); }
		#endregion
		#region Conversion
		public static QuantityValue<Pressure> ToPascals(this QuantityValue<Pressure> value) { return ConvertValue(value, pascalIndex); }
		public static QuantityValue<Pressure> ToAtmospheres(this QuantityValue<Pressure> value) { return ConvertValue(value, atmosphereIndex); }
		public static QuantityValue<Pressure> ToMmHg(this QuantityValue<Pressure> value) { return ConvertValue(value, mmHgIndex); }
		public static QuantityValue<Pressure> ToPoundsPerSquareInch(this QuantityValue<Pressure> value) { return ConvertValue(value, psiIndex); }
		public static QuantityValue<Pressure> ToTorrs(this QuantityValue<Pressure> value) { return ConvertValue(value, torrIndex); }
		#endregion
	}
	#endregion
}
