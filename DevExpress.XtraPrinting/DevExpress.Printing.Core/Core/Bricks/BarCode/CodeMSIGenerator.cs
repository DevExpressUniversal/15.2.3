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
using System.Text;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BarCode.Native;
namespace DevExpress.XtraPrinting.BarCode {
	public class CodeMSIGenerator : BarCodeGeneratorBase {
		#region static
		static string validCharSet = charSetDigits;
		static Hashtable charPattern = new Hashtable();
		static char CalcCheckDigit(string text) {
			int count = text.Length;
			if(count <= 0)
				return '0';
			StringBuilder oddNumberBuilder = new StringBuilder();
			bool oddDigit = true;
			int evenSum = 0;
			for(int i = count - 1; i >= 0; i--) {
				if(oddDigit) {
					oddNumberBuilder.Insert(0, text[i]);
				} else {
					evenSum += Char2Int(text[i]);
				}
				oddDigit = !oddDigit;
			}
			string oddNumber = oddNumberBuilder.ToString();
			oddNumber = Mul2(oddNumber);
			int oddSum = 0;
			count = oddNumber.Length;
			for(int i = 0; i < count; i++)
				oddSum += Char2Int(oddNumber[i]);
			int sum = oddSum + evenSum;
			sum %= 10;
			if(sum != 0)
				sum = 10 - sum;
			return (char)((int)'0' + sum);
		}
		#endregion
		#region Fields & Properties
		protected const DevExpress.XtraPrinting.BarCode.MSICheckSum defaultCheckSum = DevExpress.XtraPrinting.BarCode.MSICheckSum.Modulo10;
		DevExpress.XtraPrinting.BarCode.MSICheckSum checkSum = defaultCheckSum;
		[
		DefaultValue(true),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool CalcCheckSum {
			get { return checkSum != DevExpress.XtraPrinting.BarCode.MSICheckSum.None; }
			set { }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("CodeMSIGeneratorMSICheckSum"),
#endif
	   DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.CodeMSIGenerator.MSICheckSum"),
		DefaultValue(defaultCheckSum),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public DevExpress.XtraPrinting.BarCode.MSICheckSum MSICheckSum {
			get { return checkSum; }
			set { checkSum = value; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("CodeMSIGeneratorSymbologyCode")]
#endif
public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.CodeMSI; }
		}
		#endregion
		#region static XRCodeMSIGenerator()
		static CodeMSIGenerator() {
			charPattern['0'] = "nwnwnwnw";
			charPattern['1'] = "nwnwnwwn";
			charPattern['2'] = "nwnwwnnw";
			charPattern['3'] = "nwnwwnwn";
			charPattern['4'] = "nwwnnwnw";
			charPattern['5'] = "nwwnnwwn";
			charPattern['6'] = "nwwnwnnw";
			charPattern['7'] = "nwwnwnwn";
			charPattern['8'] = "wnnwnwnw";
			charPattern['9'] = "wnnwnwwn";
			charPattern['*'] = "wn";
			charPattern['|'] = "nwn";
		}
		#endregion
		public CodeMSIGenerator() {
		}
		CodeMSIGenerator(CodeMSIGenerator source)
			: base(source) {
			checkSum = source.checkSum;
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new CodeMSIGenerator(this);
		}
		protected internal static string Mul2(string number) {
			int count = number.Length;
			StringBuilder sb = new StringBuilder(count);
			int carry = 0;
			for(int i = count - 1; i >= 0; i--) {
				int val = Char2Int(number[i]) * 2 + carry;
				carry = (val >= 10) ? 1 : 0;
				int units = carry > 0 ? val - 10 : val;
				sb.Insert(0, (char)((int)'0' + units));
			}
			if(carry > 0)
				sb.Insert(0, '1');
			return sb.ToString();
		}
		string CalculateCheckSum(string text) {
			string sum = String.Empty;
			switch(checkSum) {
				case DevExpress.XtraPrinting.BarCode.MSICheckSum.Modulo10:
					sum = CalcCheckDigit(text).ToString();
					break;
				case DevExpress.XtraPrinting.BarCode.MSICheckSum.DoubleModulo10: {
						sum = CalcCheckDigit(text).ToString();
						sum += CalcCheckDigit(text + sum).ToString();
						break;
					}
			}
			return sum;
		}
		protected override char[] PrepareText(string text) {
			if(CalcCheckSum)
				text += CalculateCheckSum(text);
			return ('*' + text + '|').ToCharArray();
		}
		protected override Hashtable GetPatternTable() {
			return charPattern;
		}
		protected override float GetPatternWidth(char pattern) {
			return pattern == 'w' ? 2 : 1;
		}
		protected override string GetValidCharSet() { return validCharSet; }
	}
}
