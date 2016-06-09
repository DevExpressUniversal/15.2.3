#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using DevExpress.Office.Utils;
using System.Diagnostics;
namespace DevExpress.Office.NumberConverters {
	#region RomanNumberConverter (abstract class)
	public abstract class RomanNumberConverter : OrdinalBasedNumberConverter {
		protected internal abstract int[] Arabics { get; }
		protected internal abstract string[] Romans { get; }
		public override string ConvertNumberCore(long value) {
			System.Diagnostics.Debug.Assert(Romans.Length == Arabics.Length);
			string result = String.Empty;
			for (int i = Romans.Length - 1; i >= 0; i--)
				while (value >= Arabics[i]) {
					value -= Arabics[i];
					result += Romans[i];
				}
			return result;
		}
	}
	#endregion
	#region UpperRomanNumberConverterClassic
	public class UpperRomanNumberConverterClassic : RomanNumberConverter {
		static int[] arabics = { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000 };
		static string[] romans = { "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M" };
		protected internal override NumberingFormat Type { get { return NumberingFormat.UpperRoman; } }
		protected internal override string[] Romans { get { return romans; } }
		protected internal override int[] Arabics { get { return arabics; } }
	}
	#endregion
	#region LowerRomanNumberConverterClassic
	public class LowerRomanNumberConverterClassic : RomanNumberConverter {
		static int[] arabics = { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000 };
		static string[] romans = { "i", "iv", "v", "ix", "x", "xl", "l", "xc", "c", "cd", "d", "cm", "m" };
		protected internal override NumberingFormat Type { get { return NumberingFormat.LowerRoman; } }
		protected internal override string[] Romans { get { return romans; } }
		protected internal override int[] Arabics { get { return arabics; } }
	}
	#endregion
	#region UpperRomanNumberConverterAlternative_x45x99
	public class UpperRomanNumberConverterAlternative_x45x99 : RomanNumberConverter {
		int[] arabics = { 1, 4, 5, 9, 10, 40, 45, 95, 400, 450, 900, 950 };
		string[] romans = { "I", "IV", "V", "IX", "X", "XL", "VL", "VC", "CD", "LD", "CM", "LM" };
		protected internal override NumberingFormat Type { get { return NumberingFormat.UpperRoman; } }
		protected internal override string[] Romans { get { return romans; } }
		protected internal override int[] Arabics { get { return arabics; } }
	}
	#endregion
	#region LowerRomanNumberConverterAlternative_x45x99
	public class LowerRomanNumberConverterAlternative_x45x99 : RomanNumberConverter {
		int[] arabics = { 1, 4, 5, 9, 10, 40, 45, 95, 400, 450, 900, 950 };
		string[] romans = { "i", "iv", "v", "ix", "x", "xl", "vl", "vc", "cd", "ld", "cm", "lm" };
		protected internal override NumberingFormat Type { get { return NumberingFormat.LowerRoman; } }
		protected internal override string[] Romans { get { return romans; } }
		protected internal override int[] Arabics { get { return arabics; } }
	}
	#endregion
	#region UpperRomanNumberConverterAlternative_x90x99
	public class UpperRomanNumberConverterAlternative_x90x99 : RomanNumberConverter {
		int[] arabics = { 1, 4, 5, 9, 490, 990 };
		string[] romans = { "I", "IV", "V", "IX", "XD", "XM" };
		protected internal override NumberingFormat Type { get { return NumberingFormat.UpperRoman; } }
		protected internal override string[] Romans { get { return romans; } }
		protected internal override int[] Arabics { get { return arabics; } }
	}
	#endregion
	#region LowerRomanNumberConverterAlternative_x90x99
	public class LowerRomanNumberConverterAlternative_x90x99 : RomanNumberConverter {
		int[] arabics = { 1, 4, 5, 9, 490, 990 };
		string[] romans = { "i", "iv", "v", "ix", "xd", "xm" };
		protected internal override NumberingFormat Type { get { return NumberingFormat.LowerRoman; } }
		protected internal override string[] Romans { get { return romans; } }
		protected internal override int[] Arabics { get { return arabics; } }
	}
	#endregion
	#region UpperRomanNumberConverterAlternative_x95x99
	public class UpperRomanNumberConverterAlternative_x95x99 : RomanNumberConverter {
		int[] arabics = { 1, 4, 495, 995 };
		string[] romans = { "I", "IV", "VD", "VM" };
		protected internal override NumberingFormat Type { get { return NumberingFormat.UpperRoman; } }
		protected internal override string[] Romans { get { return romans; } }
		protected internal override int[] Arabics { get { return arabics; } }
	}
	#endregion
	#region LowerRomanNumberConverterAlternative_x95x99
	public class LowerRomanNumberConverterAlternative_x95x99 : RomanNumberConverter {
		int[] arabics = { 1, 4, 495, 995 };
		string[] romans = { "i", "iv", "vd", "vm" };
		protected internal override NumberingFormat Type { get { return NumberingFormat.LowerRoman; } }
		protected internal override string[] Romans { get { return romans; } }
		protected internal override int[] Arabics { get { return arabics; } }
	}
	#endregion
	#region UpperRomanNumberConverterAlternative_x99
	public class UpperRomanNumberConverterAlternative_x99 : RomanNumberConverter {
		int[] arabics = { 49, 99, 499, 999 };
		string[] romans = { "IL", "IC", "ID", "IM" };
		protected internal override NumberingFormat Type { get { return NumberingFormat.UpperRoman; } }
		protected internal override string[] Romans { get { return romans; } }
		protected internal override int[] Arabics { get { return arabics; } }
	}
	#endregion
	#region LowerRomanNumberConverterAlternative_x99
	public class LowerRomanNumberConverterAlternative_x99 : RomanNumberConverter {
		int[] arabics = { 49, 99, 499, 999 };
		string[] romans = { "il", "ic", "id", "im" };
		protected internal override NumberingFormat Type { get { return NumberingFormat.LowerRoman; } }
		protected internal override string[] Romans { get { return romans; } }
		protected internal override int[] Arabics { get { return arabics; } }
	}
	#endregion
}
