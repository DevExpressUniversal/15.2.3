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
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BarCode.Native;
using DevExpress.Data;
namespace DevExpress.XtraPrinting.BarCode {
	public class Industrial2of5Generator : BarCodeGeneratorBase {
		protected const float defaultWideNarrowRatio = 2.5f;
		static string validCharSet = charSetDigits;
		static Hashtable charPattern = new Hashtable();
		#region Fields & Properties
		float wideNarrowRatio = defaultWideNarrowRatio;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("Industrial2of5GeneratorWideNarrowRatio"),
#endif
	   DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraPrinting.BarCode.Industrial2of5Generator.WideNarrowRatio"),
		DefaultValue(defaultWideNarrowRatio),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public float WideNarrowRatio {
			get { return wideNarrowRatio; }
			set {
				if (value < 2.5f)
					value = 2.5f;
				wideNarrowRatio = value;
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("Industrial2of5GeneratorSymbologyCode")]
#endif
public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.Industrial2of5; }
		}
		#endregion
		#region static XRIndustrial2of5Generator()
		static Industrial2of5Generator() {
			charPattern['0'] = "nnnnwnwnnn";
			charPattern['1'] = "wnnnnnnnwn";
			charPattern['2'] = "nnwnnnnnwn";
			charPattern['3'] = "wnwnnnnnnn";
			charPattern['4'] = "nnnnwnnnwn";
			charPattern['5'] = "wnnnwnnnnn";
			charPattern['6'] = "nnwnwnnnnn";
			charPattern['7'] = "nnnnnnwnwn";
			charPattern['8'] = "wnnnnnwnnn";
			charPattern['9'] = "nnwnnnwnnn";
			charPattern['B'] = "wnwnnn";
			charPattern['E'] = "wnnnwn";
		}
		#endregion
		public Industrial2of5Generator() {
		}
		protected Industrial2of5Generator(Industrial2of5Generator source)
			: base(source) {
			wideNarrowRatio = source.wideNarrowRatio;
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new Industrial2of5Generator(this);
		}
		protected override Hashtable GetPatternTable() {
			return charPattern;
		}
		protected override float GetPatternWidth(char pattern) {
			return pattern == 'n' ? 1 : wideNarrowRatio;
		}
		protected internal static char CalcCheckDigit(string text) {
			int evenSum = 0;
			int oddSum = 0;
			int count = text.Length;
			for(int i = 0; i < count; i++) {
				if(i % 2 == 0)
					evenSum += Char2Int(text[i]);
				else
					oddSum += Char2Int(text[i]);
			}
			if(count % 2 == 0) {
				int temp = evenSum;
				evenSum = oddSum;
				oddSum = temp;
			}
			evenSum *= 3;
			evenSum += oddSum;
			evenSum %= 10;
			if(evenSum != 0)
				evenSum = 10 - evenSum;
			return (char)(evenSum + (int)'0');
		}
		protected override string FormatText(string text) {
			if(CalcCheckSum)
				text += CalcCheckDigit(text);
			return text;
		}
		protected override char[] PrepareText(string text) {
			return ('B' + text + 'E').ToCharArray();
		}
		protected override string GetValidCharSet() { return validCharSet; }
	}
}
