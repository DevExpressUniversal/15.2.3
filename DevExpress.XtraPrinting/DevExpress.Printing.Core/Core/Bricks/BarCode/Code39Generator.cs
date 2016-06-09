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
namespace DevExpress.XtraPrinting.BarCode {
	public class Code39Generator : BarCodeGeneratorBase {
		protected const int defaultWideNarrowRatio = 3;
		static string charIndexes = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%";
		static Hashtable charPattern = new Hashtable();
		#region Fields & Properties
		float wideNarrowRatio = defaultWideNarrowRatio;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("Code39GeneratorWideNarrowRatio"),
#endif
	 DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.Code39Generator.WideNarrowRatio"),
	   DefaultValue(defaultWideNarrowRatio),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public float WideNarrowRatio {
			get { return wideNarrowRatio; }
			set {
				if (value < 2.0f)
					value = 2.0f;
				if (value > 3.0f)
					value = 3.0f;
				wideNarrowRatio = value;
			}
		}
		#endregion
		#region static XRCode39Generator()
		static Code39Generator() {
			charPattern['1'] = "wnnwnnnnwn";
			charPattern['2'] = "nnwwnnnnwn";
			charPattern['3'] = "wnwwnnnnnn";
			charPattern['4'] = "nnnwwnnnwn";
			charPattern['5'] = "wnnwwnnnnn";
			charPattern['6'] = "nnwwwnnnnn";
			charPattern['7'] = "nnnwnnwnwn";
			charPattern['8'] = "wnnwnnwnnn";
			charPattern['9'] = "nnwwnnwnnn";
			charPattern['0'] = "nnnwwnwnnn";
			charPattern['A'] = "wnnnnwnnwn";
			charPattern['B'] = "nnwnnwnnwn";
			charPattern['C'] = "wnwnnwnnnn";
			charPattern['D'] = "nnnnwwnnwn";
			charPattern['E'] = "wnnnwwnnnn";
			charPattern['F'] = "nnwnwwnnnn";
			charPattern['G'] = "nnnnnwwnwn";
			charPattern['H'] = "wnnnnwwnnn";
			charPattern['I'] = "nnwnnwwnnn";
			charPattern['J'] = "nnnnwwwnnn";
			charPattern['K'] = "wnnnnnnwwn";
			charPattern['L'] = "nnwnnnnwwn";
			charPattern['M'] = "wnwnnnnwnn";
			charPattern['N'] = "nnnnwnnwwn";
			charPattern['O'] = "wnnnwnnwnn";
			charPattern['P'] = "nnwnwnnwnn";
			charPattern['Q'] = "nnnnnnwwwn";
			charPattern['R'] = "wnnnnnwwnn";
			charPattern['S'] = "nnwnnnwwnn";
			charPattern['T'] = "nnnnwnwwnn";
			charPattern['U'] = "wwnnnnnnwn";
			charPattern['V'] = "nwwnnnnnwn";
			charPattern['W'] = "wwwnnnnnnn";
			charPattern['X'] = "nwnnwnnnwn";
			charPattern['Y'] = "wwnnwnnnnn";
			charPattern['Z'] = "nwwnwnnnnn";
			charPattern['-'] = "nwnnnnwnwn";
			charPattern['.'] = "wwnnnnwnnn";
			charPattern[' '] = "nwwnnnwnnn";
			charPattern['*'] = "nwnnwnwnnn";
			charPattern['$'] = "nwnwnwnnnn";
			charPattern['/'] = "nwnwnnnwnn";
			charPattern['+'] = "nwnnnwnwnn";
			charPattern['%'] = "nnnwnwnwnn";
		}
		#endregion
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("Code39GeneratorSymbologyCode")]
#endif
public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.Code39; }
		}
		public Code39Generator() {
		}
		protected Code39Generator(Code39Generator source)
			: base(source) {
			wideNarrowRatio = source.wideNarrowRatio;
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new Code39Generator(this);
		}
		protected static char CalcCheckDigit(string text) {
			int sum = 0;
			int count = text.Length;
			for(int i = 0; i < count; i++)
				if(charIndexes.IndexOf(text[i]) >= 0)
					sum += charIndexes.IndexOf(text[i]);
			return charIndexes[sum % 43];
		}
		protected override Hashtable GetPatternTable() {
			return charPattern;
		}
		protected override float GetPatternWidth(char pattern) {
			return pattern == 'n' ? 1 : wideNarrowRatio;
		}
		protected override string FormatText(string text) {
			if(CalcCheckSum)
				text += CalcCheckDigit(text);
			return text;
		}
		protected override char[] PrepareText(string text) {
			return ('*' + text + '*').ToCharArray();
		}
		protected override string GetValidCharSet() { return charIndexes; }
	}
}
