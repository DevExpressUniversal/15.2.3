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
	#region Magnetism
	public enum Magnetism {
		Tesla,
		Gauss,
	}
	#endregion
	#region MagnetismUnitsConverter
	public class MagnetismUnitsConverter : BaseUnitsConverter<Magnetism> {
		static readonly Dictionary<Magnetism, string> unitNameTable = CreateUnitNameTable();
		static Dictionary<Magnetism, string> CreateUnitNameTable() {
			Dictionary<Magnetism, string> result = new Dictionary<Magnetism, string>();
			result.Add(Magnetism.Tesla, "T");
			result.Add(Magnetism.Gauss, "ga");
			return result;
		}
		protected override UnitCategory Category { get { return FunctionConvert.Magnetism; } }
		protected override Dictionary<Magnetism, string> UnitNameTable { get { return unitNameTable; } }
	}
	#endregion
	#region MagnetismExtensions
	public static class MagnetismExtensions {
		static readonly UnitCategory category = FunctionConvert.Magnetism;
		static UnitCategory Category { get { return category; } }
		static readonly int teslaIndex = Category.UnitIndices["T"];
		static readonly int gaussIndex = Category.UnitIndices["ga"];
		static QuantityValue<Magnetism> CreateValue(double value, int index) {
			QuantityValue<Magnetism> result = new QuantityValue<Magnetism>();
			result.Value = value;
			result.Category = Category;
			result.Index = index;
			return result;
		}
		static QuantityValue<Magnetism> ConvertValue(QuantityValue<Magnetism> value, int index) {
			return CreateValue(Category.Convert(value.Value, value.Index, index), index);
		}
		#region Creation (double)
		public static QuantityValue<Magnetism> Tesla(this double value) { return CreateValue(value, teslaIndex); }
		public static QuantityValue<Magnetism> Gauss(this double value) { return CreateValue(value, gaussIndex); }
		#endregion
		#region Creation (float)
		public static QuantityValue<Magnetism> Tesla(this float value) { return CreateValue(value, teslaIndex); }
		public static QuantityValue<Magnetism> Gauss(this float value) { return CreateValue(value, gaussIndex); }
		#endregion
		#region Creation (int)
		public static QuantityValue<Magnetism> Tesla(this int value) { return CreateValue(value, teslaIndex); }
		public static QuantityValue<Magnetism> Gauss(this int value) { return CreateValue(value, gaussIndex); }
		#endregion
		#region Conversion
		public static QuantityValue<Magnetism> ToTesla(this QuantityValue<Magnetism> value) { return ConvertValue(value, teslaIndex); }
		public static QuantityValue<Magnetism> ToGauss(this QuantityValue<Magnetism> value) { return ConvertValue(value, gaussIndex); }
		#endregion
	}
	#endregion
}
