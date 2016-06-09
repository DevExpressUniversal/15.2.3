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
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraPrinting.BarCode.Native;
namespace DevExpress.XtraPrinting.BarCode {
	public class Code93Generator : BarCodeGeneratorBase {
		#region static
		static string validCharSet = charSetDigits + charSetUpperCase + "-. $/+%";
		static string charIndexes = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%?:!~*";
		static Hashtable charPattern = new Hashtable();
		static Code93Generator() {
			charPattern['0'] = "131112"; 
			charPattern['1'] = "111213"; 
			charPattern['2'] = "111312"; 
			charPattern['3'] = "111411"; 
			charPattern['4'] = "121113"; 
			charPattern['5'] = "121212"; 
			charPattern['6'] = "121311"; 
			charPattern['7'] = "111114"; 
			charPattern['8'] = "131211"; 
			charPattern['9'] = "141111"; 
			charPattern['A'] = "211113"; 
			charPattern['B'] = "211212"; 
			charPattern['C'] = "211311"; 
			charPattern['D'] = "221112"; 
			charPattern['E'] = "221211"; 
			charPattern['F'] = "231111"; 
			charPattern['G'] = "112113"; 
			charPattern['H'] = "112212"; 
			charPattern['I'] = "112311"; 
			charPattern['J'] = "122112"; 
			charPattern['K'] = "132111"; 
			charPattern['L'] = "111123"; 
			charPattern['M'] = "111222"; 
			charPattern['N'] = "111321"; 
			charPattern['O'] = "121122"; 
			charPattern['P'] = "131121"; 
			charPattern['Q'] = "212112"; 
			charPattern['R'] = "212211"; 
			charPattern['S'] = "211122"; 
			charPattern['T'] = "211221"; 
			charPattern['U'] = "221121"; 
			charPattern['V'] = "222111"; 
			charPattern['W'] = "112122"; 
			charPattern['X'] = "112221"; 
			charPattern['Y'] = "122121"; 
			charPattern['Z'] = "123111"; 
			charPattern['-'] = "121131"; 
			charPattern['.'] = "311112"; 
			charPattern[' '] = "311211"; 
			charPattern['$'] = "321111"; 
			charPattern['/'] = "112131"; 
			charPattern['+'] = "113121"; 
			charPattern['%'] = "211131"; 
			charPattern['?'] = "121221"; 
			charPattern[':'] = "312111"; 
			charPattern['!'] = "311121"; 
			charPattern['~'] = "122211"; 
			charPattern['*'] = "111141"; 
			charPattern['|'] = "1111411"; 
		}
		static char CalcCCheckDigit(string text) {
			return CalcCheckDigit(text, charIndexes, 20, 47);
		}
		static char CalcKCheckDigit(string text) {
			return CalcCheckDigit(text, charIndexes, 15, 47);
		}
		#endregion
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("Code93GeneratorSymbologyCode")]
#endif
		public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.Code93; }
		}
		public Code93Generator() {
		}
		protected Code93Generator(Code93Generator source)
			: base(source) {
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new Code93Generator(this);
		}
		protected internal static char CalcCheckDigit(string text, string charIndexes, int weighting, int modulo) {
			int checkSum = 0;
			int maxIndex = text.Length - 1;
			int weight = 1;
			for(int i = maxIndex; i >= 0; i--) {
				if(charIndexes.IndexOf(text[i]) >= 0) {
					checkSum += charIndexes.IndexOf(text[i]) * weight;
					if(weight == weighting)
						weight = 1;
					else
						weight++;
				}
			}
			return charIndexes[checkSum % modulo];
		}
		protected override char[] PrepareText(string text) {
			if(CalcCheckSum) {
				text += CalcCCheckDigit(text);
				text += CalcKCheckDigit(text);
			}
			return ('*' + text + '|').ToCharArray();
		}
		protected override Hashtable GetPatternTable() {
			return charPattern;
		}
		protected override string GetValidCharSet() { return validCharSet; }
	}
}
