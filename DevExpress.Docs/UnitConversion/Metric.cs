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
	#region MetricPrefix
	public enum MetricPrefix {
		Yotta,
		Zetta,
		Exa,
		Peta,
		Tera,
		Giga,
		Mega,
		Kilo,
		Hecto,
		Dekao,
		Deci,
		None, 
		Centi,
		Milli,
		Micro,
		Nano,
		Pico,
		Femto,
		Atto,
		Zepto,
		Yocto,
	}
	#endregion
	#region MetricUnitsConverter
	public class MetricUnitsConverter : PrefixUnitsConverter<MetricPrefix> {
		static readonly Dictionary<MetricPrefix, string> unitNameTable = CreateUnitNameTable();
		static Dictionary<MetricPrefix, string> CreateUnitNameTable() {
			Dictionary<MetricPrefix, string> result = new Dictionary<MetricPrefix, string>();
			result.Add(MetricPrefix.Yotta, "Y");
			result.Add(MetricPrefix.Zetta, "Z");
			result.Add(MetricPrefix.Exa, "E");
			result.Add(MetricPrefix.Peta, "P");
			result.Add(MetricPrefix.Tera, "T");
			result.Add(MetricPrefix.Giga, "G");
			result.Add(MetricPrefix.Mega, "M");
			result.Add(MetricPrefix.Kilo, "k");
			result.Add(MetricPrefix.Hecto, "h");
			result.Add(MetricPrefix.Dekao, "e");
			result.Add(MetricPrefix.Deci, "d");
			result.Add(MetricPrefix.Centi, "c");
			result.Add(MetricPrefix.Milli, "m");
			result.Add(MetricPrefix.Micro, "u");
			result.Add(MetricPrefix.Nano, "n");
			result.Add(MetricPrefix.Pico, "p");
			result.Add(MetricPrefix.Femto, "f");
			result.Add(MetricPrefix.Atto, "a");
			result.Add(MetricPrefix.Zepto, "z");
			result.Add(MetricPrefix.Yocto, "y");
			return result;
		}
		protected override Dictionary<string, double> Multipliers { get { return FunctionConvert.Metric; } }
		protected override Dictionary<MetricPrefix, string> UnitNameTable { get { return unitNameTable; } }
		protected override MetricPrefix Empty { get { return MetricPrefix.None; } }
	}
	#endregion
}
