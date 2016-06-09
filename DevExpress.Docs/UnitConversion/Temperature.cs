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

using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.UnitConversion {
	#region Temperature
	public enum Temperature {
		Celsius, 
		[Obsolete("Use the Celsius instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
		Celcius = Celsius, 
		Kelvin, 
		Fahrenheit, 
		Rankine,
		Reaumur,
		C,
		K,
		F,
	}
	#endregion
	#region TemperatureUnitsConverter
	public class TemperatureUnitsConverter : BaseUnitsConverter<Temperature> {
		static readonly Dictionary<Temperature, string> unitNameTable = CreateUnitNameTable();
		static Dictionary<Temperature, string> CreateUnitNameTable() {
			Dictionary<Temperature, string> result = new Dictionary<Temperature, string>();
			result.Add(Temperature.Celsius, "C");
			result.Add(Temperature.Kelvin, "K");
			result.Add(Temperature.Fahrenheit, "F");
			result.Add(Temperature.C, "C");
			result.Add(Temperature.K, "K");
			result.Add(Temperature.F, "F");
			result.Add(Temperature.Rankine, "Rank");
			result.Add(Temperature.Reaumur, "Reau");
			return result;
		}
		protected override UnitCategory Category { get { return FunctionConvert.Temperature; } }
		protected override Dictionary<Temperature, string> UnitNameTable { get { return unitNameTable; } }
	}
	#endregion
	#region TemperatureExtensions
	public static class TemperatureExtensions {
		static readonly UnitCategory category = FunctionConvert.Temperature;
		static UnitCategory Category { get { return category; } }
		static readonly int kelvinIndex = Category.UnitIndices["K"];
		static readonly int celsiusIndex = Category.UnitIndices["C"];
		static readonly int fahrenheitIndex = Category.UnitIndices["F"];
		static readonly int rankineIndex = Category.UnitIndices["Rank"];
		static readonly int reaumurIndex = Category.UnitIndices["Reau"];
		static QuantityValue<Temperature> CreateValue(double value, int index) {
			QuantityValue<Temperature> result = new QuantityValue<Temperature>();
			result.Value = value;
			result.Category = Category;
			result.Index = index;
			return result;
		}
		static QuantityValue<Temperature> ConvertValue(QuantityValue<Temperature> value, int index) {
			return CreateValue(Category.Convert(value.Value, value.Index, index), index);
		}
		#region Creation (double)
		public static QuantityValue<Temperature> Kelvin(this double value) { return CreateValue(value, kelvinIndex); }
		[Obsolete("Use the Celsius method instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
		public static QuantityValue<Temperature> Celcius(this double value) { return CreateValue(value, celsiusIndex); }
		public static QuantityValue<Temperature> Celsius(this double value) { return CreateValue(value, celsiusIndex); }
		public static QuantityValue<Temperature> Fahrenheit(this double value) { return CreateValue(value, fahrenheitIndex); }
		public static QuantityValue<Temperature> Rankine(this double value) { return CreateValue(value, rankineIndex); }
		public static QuantityValue<Temperature> Reaumur(this double value) { return CreateValue(value, reaumurIndex); }
		#endregion
		#region Creation (float)
		public static QuantityValue<Temperature> Kelvin(this float value) { return CreateValue(value, kelvinIndex); }
		[Obsolete("Use the Celsius method instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
		public static QuantityValue<Temperature> Celcius(this float value) { return CreateValue(value, celsiusIndex); }
		public static QuantityValue<Temperature> Celsius(this float value) { return CreateValue(value, celsiusIndex); }
		public static QuantityValue<Temperature> Fahrenheit(this float value) { return CreateValue(value, fahrenheitIndex); }
		public static QuantityValue<Temperature> Rankine(this float value) { return CreateValue(value, rankineIndex); }
		public static QuantityValue<Temperature> Reaumur(this float value) { return CreateValue(value, reaumurIndex); }
		#endregion
		#region Creation (int)
		public static QuantityValue<Temperature> Kelvin(this int value) { return CreateValue(value, kelvinIndex); }
		[Obsolete("Use the Celsius method instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
		public static QuantityValue<Temperature> Celcius(this int value) { return CreateValue(value, celsiusIndex); }
		public static QuantityValue<Temperature> Celsius(this int value) { return CreateValue(value, celsiusIndex); }
		public static QuantityValue<Temperature> Fahrenheit(this int value) { return CreateValue(value, fahrenheitIndex); }
		public static QuantityValue<Temperature> Rankine(this int value) { return CreateValue(value, rankineIndex); }
		public static QuantityValue<Temperature> Reaumur(this int value) { return CreateValue(value, reaumurIndex); }
		#endregion
		#region Conversion
		public static QuantityValue<Temperature> ToKelvin(this QuantityValue<Temperature> value) { return ConvertValue(value, kelvinIndex); }
		[Obsolete("Use the ToCelsius method instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
		public static QuantityValue<Temperature> ToCelcius(this QuantityValue<Temperature> value) { return ConvertValue(value, celsiusIndex); }
		public static QuantityValue<Temperature> ToCelsius(this QuantityValue<Temperature> value) { return ConvertValue(value, celsiusIndex); }
		public static QuantityValue<Temperature> ToFahrenheit(this QuantityValue<Temperature> value) { return ConvertValue(value, fahrenheitIndex); }
		public static QuantityValue<Temperature> ToRankine(this QuantityValue<Temperature> value) { return ConvertValue(value, rankineIndex); }
		public static QuantityValue<Temperature> ToReaumur(this QuantityValue<Temperature> value) { return ConvertValue(value, reaumurIndex); }
		#endregion
	}
	#endregion
}
