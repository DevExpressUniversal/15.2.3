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
	#region BinaryPrefix
	public enum BinaryPrefix {
		Yobi,
		Zebi,
		Exbi,
		Pebi,
		Tebi,
		Gibi,
		Mebi,
		Kibi,
		Yotta,
		Zetta,
		Exa,
		Peta,
		Tera,
		Giga,
		Mega,
		Kilo,
		None,
	}
	#endregion
	#region BinaryUnitsConverter
	public class BinaryUnitsConverter : PrefixUnitsConverter<BinaryPrefix> {
		static readonly Dictionary<BinaryPrefix, string> unitNameTable = CreateUnitNameTable();
		static Dictionary<BinaryPrefix, string> CreateUnitNameTable() {
			Dictionary<BinaryPrefix, string> result = new Dictionary<BinaryPrefix, string>();
			result.Add(BinaryPrefix.Yobi, "Yi");
			result.Add(BinaryPrefix.Yotta, "Yi");
			result.Add(BinaryPrefix.Zebi, "Zi");
			result.Add(BinaryPrefix.Zetta, "Zi");
			result.Add(BinaryPrefix.Exbi, "Ei");
			result.Add(BinaryPrefix.Exa, "Ei");
			result.Add(BinaryPrefix.Pebi, "Pi");
			result.Add(BinaryPrefix.Peta, "Pi");
			result.Add(BinaryPrefix.Tebi, "Ti");
			result.Add(BinaryPrefix.Tera, "Ti");
			result.Add(BinaryPrefix.Gibi, "Gi");
			result.Add(BinaryPrefix.Giga, "Gi");
			result.Add(BinaryPrefix.Mebi, "Mi");
			result.Add(BinaryPrefix.Mega, "Mi");
			result.Add(BinaryPrefix.Kibi, "ki");
			result.Add(BinaryPrefix.Kilo, "ki");
			return result;
		}
		protected override Dictionary<string, double> Multipliers { get { return FunctionConvert.Binary; } }
		protected override Dictionary<BinaryPrefix, string> UnitNameTable { get { return unitNameTable; } }
		protected override BinaryPrefix Empty { get { return BinaryPrefix.None; } }
	}
	#endregion
}
