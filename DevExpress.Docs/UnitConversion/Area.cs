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
	#region Area
	public enum Area {
		AcreInternational,
		AcreStatute,
		SquareAngstrom,
		Are,
		SquareFoot,
		Hectare,
		SquareInch,
		SquareLightYear,
		SquareMeter,
		Morgen,
		SquareMile,
		SquareMileNautical,
		SquarePicaPoint,
		SquareYard,
	}
	#endregion
	#region AreaUnitsConverter
	public class AreaUnitsConverter : BaseUnitsConverter<Area> {
		static readonly Dictionary<Area, string> unitNameTable = CreateUnitNameTable();
		static Dictionary<Area, string> CreateUnitNameTable() {
			Dictionary<Area, string> result = new Dictionary<Area, string>();
			result.Add(Area.AcreInternational, "uk_acre");
			result.Add(Area.AcreStatute, "us_acre");
			result.Add(Area.SquareAngstrom, "ang2");
			result.Add(Area.Are, "ar");
			result.Add(Area.SquareFoot, "ft2");
			result.Add(Area.Hectare, "ha");
			result.Add(Area.SquareInch, "in2");
			result.Add(Area.SquareLightYear, "ly2");
			result.Add(Area.SquareMeter, "m2");
			result.Add(Area.Morgen, "Morgen");
			result.Add(Area.SquareMile, "mi2");
			result.Add(Area.SquareMileNautical, "Nmi2");
			result.Add(Area.SquarePicaPoint, "Pica2");
			result.Add(Area.SquareYard, "yd2");
			return result;
		}
		protected override UnitCategory Category { get { return FunctionConvert.Area; } }
		protected override Dictionary<Area, string> UnitNameTable { get { return unitNameTable; } }
	}
	#endregion
	#region AreaExtensions
	public static class AreaExtensions {
		static readonly UnitCategory category = FunctionConvert.Area;
		static UnitCategory Category { get { return category; } }
		static readonly int acreInternationalIndex = Category.UnitIndices["uk_acre"];
		static readonly int acreStatuteIndex = Category.UnitIndices["us_acre"];
		static readonly int squareAngstromIndex = Category.UnitIndices["ang2"];
		static readonly int areIndex = Category.UnitIndices["ar"];
		static readonly int squareFootIndex = Category.UnitIndices["ft2"];
		static readonly int hectareIndex = Category.UnitIndices["ha"];
		static readonly int squareInchIndex = Category.UnitIndices["in2"];
		static readonly int squareLightYearIndex = Category.UnitIndices["ly2"];
		static readonly int squareMeterIndex = Category.UnitIndices["m2"];
		static readonly int morgenIndex = Category.UnitIndices["Morgen"];
		static readonly int squareMileIndex = Category.UnitIndices["mi2"];
		static readonly int squareMileNauticalIndex = Category.UnitIndices["Nmi2"];
		static readonly int squarePicaPointIndex = Category.UnitIndices["Pica2"];
		static readonly int squareYardIndex = Category.UnitIndices["yd2"];
		static QuantityValue<Area> CreateValue(double value, int index) {
			QuantityValue<Area> result = new QuantityValue<Area>();
			result.Value = value;
			result.Category = Category;
			result.Index = index;
			return result;
		}
		static QuantityValue<Area> ConvertValue(QuantityValue<Area> value, int index) {
			return CreateValue(Category.Convert(value.Value, value.Index, index), index);
		}
		#region Creation (double)
		public static QuantityValue<Area> AcresInternational(this double value) { return CreateValue(value, acreInternationalIndex); }
		public static QuantityValue<Area> AcresStatute(this double value) { return CreateValue(value, acreStatuteIndex); }
		public static QuantityValue<Area> SquareAngstroms(this double value) { return CreateValue(value, squareAngstromIndex); }
		public static QuantityValue<Area> Ares(this double value) { return CreateValue(value, areIndex); }
		public static QuantityValue<Area> SquareFeet(this double value) { return CreateValue(value, squareFootIndex); }
		public static QuantityValue<Area> Hectares(this double value) { return CreateValue(value, hectareIndex); }
		public static QuantityValue<Area> SquareInches(this double value) { return CreateValue(value, squareInchIndex); }
		public static QuantityValue<Area> SquareLightYears(this double value) { return CreateValue(value, squareLightYearIndex); }
		public static QuantityValue<Area> SquareMeters(this double value) { return CreateValue(value, squareMeterIndex); }
		public static QuantityValue<Area> Morgens(this double value) { return CreateValue(value, morgenIndex); }
		public static QuantityValue<Area> SquareMiles(this double value) { return CreateValue(value, squareMileIndex); }
		public static QuantityValue<Area> SquareMilesNautical(this double value) { return CreateValue(value, squareMileNauticalIndex); }
		public static QuantityValue<Area> SquarePicaPoints(this double value) { return CreateValue(value, squarePicaPointIndex); }
		public static QuantityValue<Area> SquareYards(this double value) { return CreateValue(value, squareYardIndex); }
		#endregion
		#region Creation (float)
		public static QuantityValue<Area> AcresInternational(this float value) { return CreateValue(value, acreInternationalIndex); }
		public static QuantityValue<Area> AcresStatute(this float value) { return CreateValue(value, acreStatuteIndex); }
		public static QuantityValue<Area> SquareAngstroms(this float value) { return CreateValue(value, squareAngstromIndex); }
		public static QuantityValue<Area> Ares(this float value) { return CreateValue(value, areIndex); }
		public static QuantityValue<Area> SquareFeet(this float value) { return CreateValue(value, squareFootIndex); }
		public static QuantityValue<Area> Hectares(this float value) { return CreateValue(value, hectareIndex); }
		public static QuantityValue<Area> SquareInches(this float value) { return CreateValue(value, squareInchIndex); }
		public static QuantityValue<Area> SquareLightYears(this float value) { return CreateValue(value, squareLightYearIndex); }
		public static QuantityValue<Area> SquareMeters(this float value) { return CreateValue(value, squareMeterIndex); }
		public static QuantityValue<Area> Morgens(this float value) { return CreateValue(value, morgenIndex); }
		public static QuantityValue<Area> SquareMiles(this float value) { return CreateValue(value, squareMileIndex); }
		public static QuantityValue<Area> SquareMilesNautical(this float value) { return CreateValue(value, squareMileNauticalIndex); }
		public static QuantityValue<Area> SquarePicaPoints(this float value) { return CreateValue(value, squarePicaPointIndex); }
		public static QuantityValue<Area> SquareYards(this float value) { return CreateValue(value, squareYardIndex); }
		#endregion
		#region Creation (int)
		public static QuantityValue<Area> AcresInternational(this int value) { return CreateValue(value, acreInternationalIndex); }
		public static QuantityValue<Area> AcresStatute(this int value) { return CreateValue(value, acreStatuteIndex); }
		public static QuantityValue<Area> SquareAngstroms(this int value) { return CreateValue(value, squareAngstromIndex); }
		public static QuantityValue<Area> Ares(this int value) { return CreateValue(value, areIndex); }
		public static QuantityValue<Area> SquareFeet(this int value) { return CreateValue(value, squareFootIndex); }
		public static QuantityValue<Area> Hectares(this int value) { return CreateValue(value, hectareIndex); }
		public static QuantityValue<Area> SquareInches(this int value) { return CreateValue(value, squareInchIndex); }
		public static QuantityValue<Area> SquareLightYears(this int value) { return CreateValue(value, squareLightYearIndex); }
		public static QuantityValue<Area> SquareMeters(this int value) { return CreateValue(value, squareMeterIndex); }
		public static QuantityValue<Area> Morgens(this int value) { return CreateValue(value, morgenIndex); }
		public static QuantityValue<Area> SquareMiles(this int value) { return CreateValue(value, squareMileIndex); }
		public static QuantityValue<Area> SquareMilesNautical(this int value) { return CreateValue(value, squareMileNauticalIndex); }
		public static QuantityValue<Area> SquarePicaPoints(this int value) { return CreateValue(value, squarePicaPointIndex); }
		public static QuantityValue<Area> SquareYards(this int value) { return CreateValue(value, squareYardIndex); }
		#endregion
		#region Conversion
		public static QuantityValue<Area> ToAcresInternational(this QuantityValue<Area> value) { return ConvertValue(value, acreInternationalIndex); }
		public static QuantityValue<Area> ToAcresStatute(this QuantityValue<Area> value) { return ConvertValue(value, acreStatuteIndex); }
		public static QuantityValue<Area> ToSquareAngstroms(this QuantityValue<Area> value) { return ConvertValue(value, squareAngstromIndex); }
		public static QuantityValue<Area> ToAres(this QuantityValue<Area> value) { return ConvertValue(value, areIndex); }
		public static QuantityValue<Area> ToSquareFeet(this QuantityValue<Area> value) { return ConvertValue(value, squareFootIndex); }
		public static QuantityValue<Area> ToHectares(this QuantityValue<Area> value) { return ConvertValue(value, hectareIndex); }
		public static QuantityValue<Area> ToSquareInches(this QuantityValue<Area> value) { return ConvertValue(value, squareInchIndex); }
		public static QuantityValue<Area> ToSquareLightYears(this QuantityValue<Area> value) { return ConvertValue(value, squareLightYearIndex); }
		public static QuantityValue<Area> ToSquareMeters(this QuantityValue<Area> value) { return ConvertValue(value, squareMeterIndex); }
		public static QuantityValue<Area> ToMorgens(this QuantityValue<Area> value) { return ConvertValue(value, morgenIndex); }
		public static QuantityValue<Area> ToSquareMiles(this QuantityValue<Area> value) { return ConvertValue(value, squareMileIndex); }
		public static QuantityValue<Area> ToSquareMilesNautical(this QuantityValue<Area> value) { return ConvertValue(value, squareMileNauticalIndex); }
		public static QuantityValue<Area> ToSquarePicaPoints(this QuantityValue<Area> value) { return ConvertValue(value, squarePicaPointIndex); }
		public static QuantityValue<Area> ToSquareYards(this QuantityValue<Area> value) { return ConvertValue(value, squareYardIndex); }
		#endregion
	}
	#endregion
}
