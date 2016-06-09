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
	#region Power
	public enum Power {
		HorsePower,
		Watt,
		HP,
		PS,
	}
	#endregion
	#region PowerUnitsConverter
	public class PowerUnitsConverter : BaseUnitsConverter<Power> {
		static readonly Dictionary<Power, string> unitNameTable = CreateUnitNameTable();
		static Dictionary<Power, string> CreateUnitNameTable() {
			Dictionary<Power, string> result = new Dictionary<Power, string>();
			result.Add(Power.HorsePower, "HP");
			result.Add(Power.HP, "HP");
			result.Add(Power.PS, "PS");
			result.Add(Power.Watt, "W");
			return result;
		}
		protected override UnitCategory Category { get { return FunctionConvert.Power; } }
		protected override Dictionary<Power, string> UnitNameTable { get { return unitNameTable; } }
	}
	#endregion
	#region PowerExtensions
	public static class PowerExtensions {
		static readonly UnitCategory category = FunctionConvert.Power;
		static UnitCategory Category { get { return category; } }
		static readonly int horsePowerIndex = Category.UnitIndices["HP"];
		static readonly int wattIndex = Category.UnitIndices["W"];
		static QuantityValue<Power> CreateValue(double value, int index) {
			QuantityValue<Power> result = new QuantityValue<Power>();
			result.Value = value;
			result.Category = Category;
			result.Index = index;
			return result;
		}
		static QuantityValue<Power> ConvertValue(QuantityValue<Power> value, int index) {
			return CreateValue(Category.Convert(value.Value, value.Index, index), index);
		}
		#region Creation (double)
		public static QuantityValue<Power> HorsePower(this double value) { return CreateValue(value, horsePowerIndex); }
		public static QuantityValue<Power> Watts(this double value) { return CreateValue(value, wattIndex); }
		#endregion
		#region Creation (float)
		public static QuantityValue<Power> HorsePower(this float value) { return CreateValue(value, horsePowerIndex); }
		public static QuantityValue<Power> Watts(this float value) { return CreateValue(value, wattIndex); }
		#endregion
		#region Creation (int)
		public static QuantityValue<Power> HorsePower(this int value) { return CreateValue(value, horsePowerIndex); }
		public static QuantityValue<Power> Watts(this int value) { return CreateValue(value, wattIndex); }
		#endregion
		#region Conversion
		public static QuantityValue<Power> ToHorsePower(this QuantityValue<Power> value) { return ConvertValue(value, horsePowerIndex); }
		public static QuantityValue<Power> ToWatts(this QuantityValue<Power> value) { return ConvertValue(value, wattIndex); }
		#endregion
	}
	#endregion
}
