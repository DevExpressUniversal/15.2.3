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
	#region Distance
	public enum Distance {
		Meter,
		MileStatute,
		MileNautical,
		MileUSSurvey,
		Inch,
		Yard,
		Foot,
		Angstrom,
		Ell,
		LightYear,
		Parsec,
		PicaPoint,
		Pica,
	}
	#endregion
	#region DistanceUnitsConverter
	public class DistanceUnitsConverter : BaseUnitsConverter<Distance> {
		static readonly Dictionary<Distance, string> unitNameTable = CreateUnitNameTable();
		static Dictionary<Distance, string> CreateUnitNameTable() {
			Dictionary<Distance, string> result = new Dictionary<Distance, string>();
			result.Add(Distance.Meter, "m");
			result.Add(Distance.MileStatute, "mi");
			result.Add(Distance.MileNautical, "Nmi");
			result.Add(Distance.MileUSSurvey, "survey_mi");
			result.Add(Distance.Inch, "in");
			result.Add(Distance.Yard, "yd");
			result.Add(Distance.Foot, "ft");
			result.Add(Distance.Angstrom, "ang");
			result.Add(Distance.Ell, "ell");
			result.Add(Distance.LightYear, "ly");
			result.Add(Distance.Parsec, "parsec");
			result.Add(Distance.PicaPoint, "Pica");
			result.Add(Distance.Pica, "pica");
			return result;
		}
		protected override UnitCategory Category { get { return FunctionConvert.Distance; } }
		protected override Dictionary<Distance, string> UnitNameTable { get { return unitNameTable; } }
	}
	#endregion
	#region DistanceExtensions
	public static class DistanceExtensions {
		static readonly UnitCategory category = FunctionConvert.Distance;
		static UnitCategory Category { get { return category; } }
		static readonly int meterIndex = Category.UnitIndices["m"];
		static readonly int mileStatuteIndex = Category.UnitIndices["mi"];
		static readonly int mileNauticalIndex = Category.UnitIndices["Nmi"];
		static readonly int mileUSSurveyIndex = Category.UnitIndices["survey_mi"];
		static readonly int inchIndex = Category.UnitIndices["in"];
		static readonly int yardIndex = Category.UnitIndices["yd"];
		static readonly int footIndex = Category.UnitIndices["ft"];
		static readonly int angstromIndex = Category.UnitIndices["ang"];
		static readonly int ellIndex = Category.UnitIndices["ell"];
		static readonly int lightYearIndex = Category.UnitIndices["ly"];
		static readonly int parsecIndex = Category.UnitIndices["pc"];
		static readonly int picaPointIndex = Category.UnitIndices["Pica"];
		static readonly int picaIndex = Category.UnitIndices["pica"];
		static QuantityValue<Distance> CreateValue(double value, int index) {
			QuantityValue<Distance> result = new QuantityValue<Distance>();
			result.Value = value;
			result.Category = Category;
			result.Index = index;
			return result;
		}
		static QuantityValue<Distance> ConvertValue(QuantityValue<Distance> value, int index) {
			return CreateValue(Category.Convert(value.Value, value.Index, index), index);
		}
		#region Creation (double)
		public static QuantityValue<Distance> Meters(this double value) { return CreateValue(value, meterIndex); }
		public static QuantityValue<Distance> MilesStatute(this double value) { return CreateValue(value, mileStatuteIndex); }
		public static QuantityValue<Distance> MilesNautical(this double value) { return CreateValue(value, mileNauticalIndex); }
		public static QuantityValue<Distance> MilesUSSurvey(this double value) { return CreateValue(value, mileUSSurveyIndex); }
		public static QuantityValue<Distance> Inches(this double value) { return CreateValue(value, inchIndex); }
		public static QuantityValue<Distance> Yards(this double value) { return CreateValue(value, yardIndex); }
		public static QuantityValue<Distance> Feet(this double value) { return CreateValue(value, footIndex); }
		public static QuantityValue<Distance> Angstroms(this double value) { return CreateValue(value, angstromIndex); }
		public static QuantityValue<Distance> Ells(this double value) { return CreateValue(value, ellIndex); }
		public static QuantityValue<Distance> LightYears(this double value) { return CreateValue(value, lightYearIndex); }
		public static QuantityValue<Distance> Parsecs(this double value) { return CreateValue(value, parsecIndex); }
		public static QuantityValue<Distance> PicaPoints(this double value) { return CreateValue(value, picaPointIndex); }
		public static QuantityValue<Distance> Picas(this double value) { return CreateValue(value, picaIndex); }
		#endregion
		#region Creation (float)
		public static QuantityValue<Distance> Meters(this float value) { return CreateValue(value, meterIndex); }
		public static QuantityValue<Distance> MilesStatute(this float value) { return CreateValue(value, mileStatuteIndex); }
		public static QuantityValue<Distance> MilesNautical(this float value) { return CreateValue(value, mileNauticalIndex); }
		public static QuantityValue<Distance> MilesUSSurvey(this float value) { return CreateValue(value, mileUSSurveyIndex); }
		public static QuantityValue<Distance> Inches(this float value) { return CreateValue(value, inchIndex); }
		public static QuantityValue<Distance> Yards(this float value) { return CreateValue(value, yardIndex); }
		public static QuantityValue<Distance> Feet(this float value) { return CreateValue(value, footIndex); }
		public static QuantityValue<Distance> Angstroms(this float value) { return CreateValue(value, angstromIndex); }
		public static QuantityValue<Distance> Ells(this float value) { return CreateValue(value, ellIndex); }
		public static QuantityValue<Distance> LightYears(this float value) { return CreateValue(value, lightYearIndex); }
		public static QuantityValue<Distance> Parsecs(this float value) { return CreateValue(value, parsecIndex); }
		public static QuantityValue<Distance> PicaPoints(this float value) { return CreateValue(value, picaPointIndex); }
		public static QuantityValue<Distance> Picas(this float value) { return CreateValue(value, picaIndex); }
		#endregion
		#region Creation (int)
		public static QuantityValue<Distance> Meters(this int value) { return CreateValue(value, meterIndex); }
		public static QuantityValue<Distance> MilesStatute(this int value) { return CreateValue(value, mileStatuteIndex); }
		public static QuantityValue<Distance> MilesNautical(this int value) { return CreateValue(value, mileNauticalIndex); }
		public static QuantityValue<Distance> MilesUSSurvey(this int value) { return CreateValue(value, mileUSSurveyIndex); }
		public static QuantityValue<Distance> Inches(this int value) { return CreateValue(value, inchIndex); }
		public static QuantityValue<Distance> Yards(this int value) { return CreateValue(value, yardIndex); }
		public static QuantityValue<Distance> Feet(this int value) { return CreateValue(value, footIndex); }
		public static QuantityValue<Distance> Angstroms(this int value) { return CreateValue(value, angstromIndex); }
		public static QuantityValue<Distance> Ells(this int value) { return CreateValue(value, ellIndex); }
		public static QuantityValue<Distance> LightYears(this int value) { return CreateValue(value, lightYearIndex); }
		public static QuantityValue<Distance> Parsecs(this int value) { return CreateValue(value, parsecIndex); }
		public static QuantityValue<Distance> PicaPoints(this int value) { return CreateValue(value, picaPointIndex); }
		public static QuantityValue<Distance> Picas(this int value) { return CreateValue(value, picaIndex); }
		#endregion
		#region Conversion
		public static QuantityValue<Distance> ToMeters(this QuantityValue<Distance> value) { return ConvertValue(value, meterIndex); }
		public static QuantityValue<Distance> ToMilesStatute(this QuantityValue<Distance> value) { return ConvertValue(value, mileStatuteIndex); }
		public static QuantityValue<Distance> ToMilesNautical(this QuantityValue<Distance> value) { return ConvertValue(value, mileNauticalIndex); }
		public static QuantityValue<Distance> ToMilesUSSurvey(this QuantityValue<Distance> value) { return ConvertValue(value, mileUSSurveyIndex); }
		public static QuantityValue<Distance> ToInches(this QuantityValue<Distance> value) { return ConvertValue(value, inchIndex); }
		public static QuantityValue<Distance> ToYards(this QuantityValue<Distance> value) { return ConvertValue(value, yardIndex); }
		public static QuantityValue<Distance> ToFeet(this QuantityValue<Distance> value) { return ConvertValue(value, footIndex); }
		public static QuantityValue<Distance> ToAngstroms(this QuantityValue<Distance> value) { return ConvertValue(value, angstromIndex); }
		public static QuantityValue<Distance> ToElls(this QuantityValue<Distance> value) { return ConvertValue(value, ellIndex); }
		public static QuantityValue<Distance> ToLightYears(this QuantityValue<Distance> value) { return ConvertValue(value, lightYearIndex); }
		public static QuantityValue<Distance> ToParsecs(this QuantityValue<Distance> value) { return ConvertValue(value, parsecIndex); }
		public static QuantityValue<Distance> ToPicaPoints(this QuantityValue<Distance> value) { return ConvertValue(value, picaPointIndex); }
		public static QuantityValue<Distance> ToPicas(this QuantityValue<Distance> value) { return ConvertValue(value, picaIndex); }
		#endregion
	}
	#endregion
}
