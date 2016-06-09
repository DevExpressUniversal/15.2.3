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
	#region Volume
	public enum Volume {
		Teaspoon,
		TeaspoonModern,
		Tablespoon,
		OunceFluid,
		Cup,
		Pint,
		PintImperial,
		Quart,
		QuartImperial,
		Gallon,
		GallonImperial,
		Liter,
		Bushel,
		OilBarrel,
		CubicAngstrom,
		CubicFoot,
		CubicInch,
		CubicLightYear,
		CubicMeter,
		CubicMile,
		CubicYard,
		CubicMileNautical,
		CubicPicaPoint,
		GrossRegisteredTon,
		MeasurementTon,
		Oz,
		Bbl,
		GRT,
		Regton,
		Mton
	}
	#endregion
	#region VolumeUnitsConverter
	public class VolumeUnitsConverter : BaseUnitsConverter<Volume> {
		static readonly Dictionary<Volume, string> unitNameTable = CreateUnitNameTable();
		static Dictionary<Volume, string> CreateUnitNameTable() {
			Dictionary<Volume, string> result = new Dictionary<Volume, string>();
			result.Add(Volume.Teaspoon, "tsp");
			result.Add(Volume.TeaspoonModern, "tspm");
			result.Add(Volume.Tablespoon, "tbs");
			result.Add(Volume.OunceFluid, "oz");
			result.Add(Volume.Oz, "oz");
			result.Add(Volume.Cup, "cup");
			result.Add(Volume.Pint, "pt");
			result.Add(Volume.PintImperial, "uk_pt");
			result.Add(Volume.Quart, "qt");
			result.Add(Volume.QuartImperial, "uk_qt");
			result.Add(Volume.Gallon, "gal");
			result.Add(Volume.GallonImperial, "uk_gal");
			result.Add(Volume.Liter, "lt");
			result.Add(Volume.Bushel, "bushel");
			result.Add(Volume.OilBarrel, "barrel");
			result.Add(Volume.Bbl, "barrel");
			result.Add(Volume.CubicAngstrom, "ang3");
			result.Add(Volume.CubicFoot, "ft3");
			result.Add(Volume.CubicInch, "in3");
			result.Add(Volume.CubicLightYear, "ly3");
			result.Add(Volume.CubicMeter, "m3");
			result.Add(Volume.CubicMile, "mi3");
			result.Add(Volume.CubicYard, "yd3");
			result.Add(Volume.CubicMileNautical, "Nmi3");
			result.Add(Volume.CubicPicaPoint, "Pica3");
			result.Add(Volume.GrossRegisteredTon, "GRT");
			result.Add(Volume.GRT, "GRT");
			result.Add(Volume.Regton, "GRT");
			result.Add(Volume.MeasurementTon, "MTON");
			result.Add(Volume.Mton, "MTON");
			return result;
		}
		protected override UnitCategory Category { get { return FunctionConvert.Volume; } }
		protected override Dictionary<Volume, string> UnitNameTable { get { return unitNameTable; } }
	}
	#endregion
	#region VolumeExtensions
	public static class VolumeExtensions {
		static readonly UnitCategory category = FunctionConvert.Volume;
		static UnitCategory Category { get { return category; } }
		static readonly int teaspoonIndex = Category.UnitIndices["tsp"];
		static readonly int teaspoonModernIndex = Category.UnitIndices["tspm"];
		static readonly int tablespoonIndex = Category.UnitIndices["tbs"];
		static readonly int ounceFluidIndex = Category.UnitIndices["oz"];
		static readonly int cupIndex = Category.UnitIndices["cup"];
		static readonly int pintIndex = Category.UnitIndices["pt"];
		static readonly int pintImperialIndex = Category.UnitIndices["uk_pt"];
		static readonly int quartIndex = Category.UnitIndices["qt"];
		static readonly int quartImperialIndex = Category.UnitIndices["uk_qt"];
		static readonly int gallonIndex = Category.UnitIndices["gal"];
		static readonly int gallonImperialIndex = Category.UnitIndices["uk_gal"];
		static readonly int literIndex = Category.UnitIndices["lt"];
		static readonly int bushelIndex = Category.UnitIndices["bushel"];
		static readonly int oilBarrelIndex = Category.UnitIndices["barrel"];
		static readonly int cubicAngstromIndex = Category.UnitIndices["ang3"];
		static readonly int cubicFootIndex = Category.UnitIndices["ft3"];
		static readonly int cubicInchIndex = Category.UnitIndices["in3"];
		static readonly int cubicLightYearIndex = Category.UnitIndices["ly3"];
		static readonly int cubicMeterIndex = Category.UnitIndices["m3"];
		static readonly int cubicMileIndex = Category.UnitIndices["mi3"];
		static readonly int cubicYardIndex = Category.UnitIndices["yd3"];
		static readonly int cubicMileNauticalIndex = Category.UnitIndices["Nmi3"];
		static readonly int cubicPicaPointIndex = Category.UnitIndices["Pica3"];
		static readonly int grossRegisteredTonIndex = Category.UnitIndices["GRT"];
		static readonly int measurementTonIndex = Category.UnitIndices["MTON"];
		static QuantityValue<Volume> CreateValue(double value, int index) {
			QuantityValue<Volume> result = new QuantityValue<Volume>();
			result.Value = value;
			result.Category = Category;
			result.Index = index;
			return result;
		}
		static QuantityValue<Volume> ConvertValue(QuantityValue<Volume> value, int index) {
			return CreateValue(Category.Convert(value.Value, value.Index, index), index);
		}
		#region Creation (double)
		public static QuantityValue<Volume> Teaspoons(this double value) { return CreateValue(value, teaspoonIndex); }
		public static QuantityValue<Volume> TeaspoonsModern(this double value) { return CreateValue(value, teaspoonModernIndex); }
		public static QuantityValue<Volume> Tablespoons(this double value) { return CreateValue(value, tablespoonIndex); }
		public static QuantityValue<Volume> OuncesFluid(this double value) { return CreateValue(value, ounceFluidIndex); }
		public static QuantityValue<Volume> Cups(this double value) { return CreateValue(value, cupIndex); }
		public static QuantityValue<Volume> Pints(this double value) { return CreateValue(value, pintIndex); }
		public static QuantityValue<Volume> PintsImperial(this double value) { return CreateValue(value, pintImperialIndex); }
		public static QuantityValue<Volume> Quarts(this double value) { return CreateValue(value, quartIndex); }
		public static QuantityValue<Volume> QuartsImperial(this double value) { return CreateValue(value, quartImperialIndex); }
		public static QuantityValue<Volume> Gallons(this double value) { return CreateValue(value, gallonIndex); }
		public static QuantityValue<Volume> GallonsImperial(this double value) { return CreateValue(value, gallonImperialIndex); }
		public static QuantityValue<Volume> Liters(this double value) { return CreateValue(value, literIndex); }
		public static QuantityValue<Volume> Bushels(this double value) { return CreateValue(value, bushelIndex); }
		public static QuantityValue<Volume> OilBarrels(this double value) { return CreateValue(value, oilBarrelIndex); }
		public static QuantityValue<Volume> CubicAngstroms(this double value) { return CreateValue(value, cubicAngstromIndex); }
		public static QuantityValue<Volume> CubicFeet(this double value) { return CreateValue(value, cubicFootIndex); }
		public static QuantityValue<Volume> CubicInches(this double value) { return CreateValue(value, cubicInchIndex); }
		public static QuantityValue<Volume> CubicLightYears(this double value) { return CreateValue(value, cubicLightYearIndex); }
		public static QuantityValue<Volume> CubicMeters(this double value) { return CreateValue(value, cubicMeterIndex); }
		public static QuantityValue<Volume> CubicMiles(this double value) { return CreateValue(value, cubicMileIndex); }
		public static QuantityValue<Volume> CubicYards(this double value) { return CreateValue(value, cubicYardIndex); }
		public static QuantityValue<Volume> CubicMilesNautical(this double value) { return CreateValue(value, cubicMileNauticalIndex); }
		public static QuantityValue<Volume> CubicPicaPoints(this double value) { return CreateValue(value, cubicPicaPointIndex); }
		public static QuantityValue<Volume> GrossRegisteredTons(this double value) { return CreateValue(value, grossRegisteredTonIndex); }
		public static QuantityValue<Volume> MeasurementTons(this double value) { return CreateValue(value, measurementTonIndex); }
		#endregion
		#region Creation (float)
		public static QuantityValue<Volume> Teaspoons(this float value) { return CreateValue(value, teaspoonIndex); }
		public static QuantityValue<Volume> TeaspoonsModern(this float value) { return CreateValue(value, teaspoonModernIndex); }
		public static QuantityValue<Volume> Tablespoons(this float value) { return CreateValue(value, tablespoonIndex); }
		public static QuantityValue<Volume> OuncesFluid(this float value) { return CreateValue(value, ounceFluidIndex); }
		public static QuantityValue<Volume> Cups(this float value) { return CreateValue(value, cupIndex); }
		public static QuantityValue<Volume> Pints(this float value) { return CreateValue(value, pintIndex); }
		public static QuantityValue<Volume> PintsImperial(this float value) { return CreateValue(value, pintImperialIndex); }
		public static QuantityValue<Volume> Quarts(this float value) { return CreateValue(value, quartIndex); }
		public static QuantityValue<Volume> QuartsImperial(this float value) { return CreateValue(value, quartImperialIndex); }
		public static QuantityValue<Volume> Gallons(this float value) { return CreateValue(value, gallonIndex); }
		public static QuantityValue<Volume> GallonsImperial(this float value) { return CreateValue(value, gallonImperialIndex); }
		public static QuantityValue<Volume> Liters(this float value) { return CreateValue(value, literIndex); }
		public static QuantityValue<Volume> Bushels(this float value) { return CreateValue(value, bushelIndex); }
		public static QuantityValue<Volume> OilBarrels(this float value) { return CreateValue(value, oilBarrelIndex); }
		public static QuantityValue<Volume> CubicAngstroms(this float value) { return CreateValue(value, cubicAngstromIndex); }
		public static QuantityValue<Volume> CubicFeet(this float value) { return CreateValue(value, cubicFootIndex); }
		public static QuantityValue<Volume> CubicInches(this float value) { return CreateValue(value, cubicInchIndex); }
		public static QuantityValue<Volume> CubicLightYears(this float value) { return CreateValue(value, cubicLightYearIndex); }
		public static QuantityValue<Volume> CubicMeters(this float value) { return CreateValue(value, cubicMeterIndex); }
		public static QuantityValue<Volume> CubicMiles(this float value) { return CreateValue(value, cubicMileIndex); }
		public static QuantityValue<Volume> CubicYards(this float value) { return CreateValue(value, cubicYardIndex); }
		public static QuantityValue<Volume> CubicMilesNautical(this float value) { return CreateValue(value, cubicMileNauticalIndex); }
		public static QuantityValue<Volume> CubicPicaPoints(this float value) { return CreateValue(value, cubicPicaPointIndex); }
		public static QuantityValue<Volume> GrossRegisteredTons(this float value) { return CreateValue(value, grossRegisteredTonIndex); }
		public static QuantityValue<Volume> MeasurementTons(this float value) { return CreateValue(value, measurementTonIndex); }
		#endregion
		#region Creation (int)
		public static QuantityValue<Volume> Teaspoons(this int value) { return CreateValue(value, teaspoonIndex); }
		public static QuantityValue<Volume> TeaspoonsModern(this int value) { return CreateValue(value, teaspoonModernIndex); }
		public static QuantityValue<Volume> Tablespoons(this int value) { return CreateValue(value, tablespoonIndex); }
		public static QuantityValue<Volume> OuncesFluid(this int value) { return CreateValue(value, ounceFluidIndex); }
		public static QuantityValue<Volume> Cups(this int value) { return CreateValue(value, cupIndex); }
		public static QuantityValue<Volume> Pints(this int value) { return CreateValue(value, pintIndex); }
		public static QuantityValue<Volume> PintsImperial(this int value) { return CreateValue(value, pintImperialIndex); }
		public static QuantityValue<Volume> Quarts(this int value) { return CreateValue(value, quartIndex); }
		public static QuantityValue<Volume> QuartsImperial(this int value) { return CreateValue(value, quartImperialIndex); }
		public static QuantityValue<Volume> Gallons(this int value) { return CreateValue(value, gallonIndex); }
		public static QuantityValue<Volume> GallonsImperial(this int value) { return CreateValue(value, gallonImperialIndex); }
		public static QuantityValue<Volume> Liters(this int value) { return CreateValue(value, literIndex); }
		public static QuantityValue<Volume> Bushels(this int value) { return CreateValue(value, bushelIndex); }
		public static QuantityValue<Volume> OilBarrels(this int value) { return CreateValue(value, oilBarrelIndex); }
		public static QuantityValue<Volume> CubicAngstroms(this int value) { return CreateValue(value, cubicAngstromIndex); }
		public static QuantityValue<Volume> CubicFeet(this int value) { return CreateValue(value, cubicFootIndex); }
		public static QuantityValue<Volume> CubicInches(this int value) { return CreateValue(value, cubicInchIndex); }
		public static QuantityValue<Volume> CubicLightYears(this int value) { return CreateValue(value, cubicLightYearIndex); }
		public static QuantityValue<Volume> CubicMeters(this int value) { return CreateValue(value, cubicMeterIndex); }
		public static QuantityValue<Volume> CubicMiles(this int value) { return CreateValue(value, cubicMileIndex); }
		public static QuantityValue<Volume> CubicYards(this int value) { return CreateValue(value, cubicYardIndex); }
		public static QuantityValue<Volume> CubicMilesNautical(this int value) { return CreateValue(value, cubicMileNauticalIndex); }
		public static QuantityValue<Volume> CubicPicaPoints(this int value) { return CreateValue(value, cubicPicaPointIndex); }
		public static QuantityValue<Volume> GrossRegisteredTons(this int value) { return CreateValue(value, grossRegisteredTonIndex); }
		public static QuantityValue<Volume> MeasurementTons(this int value) { return CreateValue(value, measurementTonIndex); }
		#endregion
		#region Conversion
		public static QuantityValue<Volume> ToTeaspoons(this QuantityValue<Volume> value) { return ConvertValue(value, teaspoonIndex); }
		public static QuantityValue<Volume> ToTeaspoonsModern(this QuantityValue<Volume> value) { return ConvertValue(value, teaspoonModernIndex); }
		public static QuantityValue<Volume> ToTablespoons(this QuantityValue<Volume> value) { return ConvertValue(value, tablespoonIndex); }
		public static QuantityValue<Volume> ToOuncesFluid(this QuantityValue<Volume> value) { return ConvertValue(value, ounceFluidIndex); }
		public static QuantityValue<Volume> ToCups(this QuantityValue<Volume> value) { return ConvertValue(value, cupIndex); }
		public static QuantityValue<Volume> ToPints(this QuantityValue<Volume> value) { return ConvertValue(value, pintIndex); }
		public static QuantityValue<Volume> ToPintsImperial(this QuantityValue<Volume> value) { return ConvertValue(value, pintImperialIndex); }
		public static QuantityValue<Volume> ToQuarts(this QuantityValue<Volume> value) { return ConvertValue(value, quartIndex); }
		public static QuantityValue<Volume> ToQuartsImperial(this QuantityValue<Volume> value) { return ConvertValue(value, quartImperialIndex); }
		public static QuantityValue<Volume> ToGallons(this QuantityValue<Volume> value) { return ConvertValue(value, gallonIndex); }
		public static QuantityValue<Volume> ToGallonsImperial(this QuantityValue<Volume> value) { return ConvertValue(value, gallonImperialIndex); }
		public static QuantityValue<Volume> ToLiters(this QuantityValue<Volume> value) { return ConvertValue(value, literIndex); }
		public static QuantityValue<Volume> ToBushels(this QuantityValue<Volume> value) { return ConvertValue(value, bushelIndex); }
		public static QuantityValue<Volume> ToOilBarrels(this QuantityValue<Volume> value) { return ConvertValue(value, oilBarrelIndex); }
		public static QuantityValue<Volume> ToCubicAngstroms(this QuantityValue<Volume> value) { return ConvertValue(value, cubicAngstromIndex); }
		public static QuantityValue<Volume> ToCubicFeet(this QuantityValue<Volume> value) { return ConvertValue(value, cubicFootIndex); }
		public static QuantityValue<Volume> ToCubicInches(this QuantityValue<Volume> value) { return ConvertValue(value, cubicInchIndex); }
		public static QuantityValue<Volume> ToCubicLightYears(this QuantityValue<Volume> value) { return ConvertValue(value, cubicLightYearIndex); }
		public static QuantityValue<Volume> ToCubicMeters(this QuantityValue<Volume> value) { return ConvertValue(value, cubicMeterIndex); }
		public static QuantityValue<Volume> ToCubicMiles(this QuantityValue<Volume> value) { return ConvertValue(value, cubicMileIndex); }
		public static QuantityValue<Volume> ToCubicYards(this QuantityValue<Volume> value) { return ConvertValue(value, cubicYardIndex); }
		public static QuantityValue<Volume> ToCubicMilesNautical(this QuantityValue<Volume> value) { return ConvertValue(value, cubicMileNauticalIndex); }
		public static QuantityValue<Volume> ToCubicPicaPoints(this QuantityValue<Volume> value) { return ConvertValue(value, cubicPicaPointIndex); }
		public static QuantityValue<Volume> ToGrossRegisteredTons(this QuantityValue<Volume> value) { return ConvertValue(value, grossRegisteredTonIndex); }
		public static QuantityValue<Volume> ToMeasurementTons(this QuantityValue<Volume> value) { return ConvertValue(value, measurementTonIndex); }
		#endregion
	}
	#endregion
}
