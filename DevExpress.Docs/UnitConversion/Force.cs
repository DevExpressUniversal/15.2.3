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
	#region Force
	public enum Force {
		Newton,
		Dyne,
		Pound,
		Pond,
		Lbf
	}
	#endregion
	#region ForceUnitsConverter
	public class ForceUnitsConverter : BaseUnitsConverter<Force> {
		static readonly Dictionary<Force, string> unitNameTable = CreateUnitNameTable();
		static Dictionary<Force, string> CreateUnitNameTable() {
			Dictionary<Force, string> result = new Dictionary<Force, string>();
			result.Add(Force.Newton, "N");
			result.Add(Force.Dyne, "dyn");
			result.Add(Force.Pound, "lbf");
			result.Add(Force.Lbf, "lbf");
			result.Add(Force.Pond, "pond");
			return result;
		}
		protected override UnitCategory Category { get { return FunctionConvert.Force; } }
		protected override Dictionary<Force, string> UnitNameTable { get { return unitNameTable; } }
	}
	#endregion
	#region ForceExtensions
	public static class ForceExtensions {
		static readonly UnitCategory category = FunctionConvert.Force;
		static UnitCategory Category { get { return category; } }
		static readonly int newtonIndex = Category.UnitIndices["N"];
		static readonly int dyneIndex = Category.UnitIndices["dy"];
		static readonly int poundForceIndex = Category.UnitIndices["lbf"];
		static readonly int pondIndex = Category.UnitIndices["pond"];
		static QuantityValue<Force> CreateValue(double value, int index) {
			QuantityValue<Force> result = new QuantityValue<Force>();
			result.Value = value;
			result.Category = Category;
			result.Index = index;
			return result;
		}
		static QuantityValue<Force> ConvertValue(QuantityValue<Force> value, int index) {
			return CreateValue(Category.Convert(value.Value, value.Index, index), index);
		}
		#region Creation (double)
		public static QuantityValue<Force> Newtons(this double value) { return CreateValue(value, newtonIndex); }
		public static QuantityValue<Force> Dynes(this double value) { return CreateValue(value, dyneIndex); }
		public static QuantityValue<Force> PoundForce(this double value) { return CreateValue(value, poundForceIndex); }
		public static QuantityValue<Force> Pond(this double value) { return CreateValue(value, pondIndex); }
		#endregion
		#region Creation (float)
		public static QuantityValue<Force> Newtons(this float value) { return CreateValue(value, newtonIndex); }
		public static QuantityValue<Force> Dynes(this float value) { return CreateValue(value, dyneIndex); }
		public static QuantityValue<Force> PoundForce(this float value) { return CreateValue(value, poundForceIndex); }
		public static QuantityValue<Force> Pond(this float value) { return CreateValue(value, pondIndex); }
		#endregion
		#region Creation (int)
		public static QuantityValue<Force> Newtons(this int value) { return CreateValue(value, newtonIndex); }
		public static QuantityValue<Force> Dynes(this int value) { return CreateValue(value, dyneIndex); }
		public static QuantityValue<Force> PoundForce(this int value) { return CreateValue(value, poundForceIndex); }
		public static QuantityValue<Force> Pond(this int value) { return CreateValue(value, pondIndex); }
		#endregion
		#region Conversion
		public static QuantityValue<Force> ToNewtons(this QuantityValue<Force> value) { return ConvertValue(value, newtonIndex); }
		public static QuantityValue<Force> ToDynes(this QuantityValue<Force> value) { return ConvertValue(value, dyneIndex); }
		public static QuantityValue<Force> ToPoundForce(this QuantityValue<Force> value) { return ConvertValue(value, poundForceIndex); }
		public static QuantityValue<Force> ToPond(this QuantityValue<Force> value) { return ConvertValue(value, pondIndex); }
		#endregion
	}
	#endregion
}
