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
	public class Code11Generator : BarCodeGeneratorBase {
		#region static
		static string validCharSet = charSetDigits + '-';
		static string charIndexes = "0123456789-";
		static Hashtable charPattern = new Hashtable();
		static Code11Generator() {
			charPattern['0'] = "nnnnwn";
			charPattern['1'] = "wnnnwn";
			charPattern['2'] = "nwnnwn";
			charPattern['3'] = "wwnnnn";
			charPattern['4'] = "nnwnwn";
			charPattern['5'] = "wnwnnn";
			charPattern['6'] = "nwwnnn";
			charPattern['7'] = "nnnwwn";
			charPattern['8'] = "wnnwnn";
			charPattern['9'] = "wnnnnn";
			charPattern['-'] = "nnwnnn";
			charPattern['*'] = "nnwwnn";
		}
		static char CalcCCheckDigit(string text) {
			return Code93Generator.CalcCheckDigit(text, charIndexes, 10, 11);
		}
		static char CalcKCheckDigit(string text) {
			return Code93Generator.CalcCheckDigit(text, charIndexes, 9, 11);
		}
		#endregion
		#region Fields & Properties
		[
		DefaultValue(true),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool CalcCheckSum {
			get { return true; }
			set { }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("Code11GeneratorSymbologyCode")]
#endif
public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.Code11; }
		}
		#endregion
		public Code11Generator() {
		}
		Code11Generator(Code11Generator source)
			: base(source) {
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new Code11Generator(this);
		}
		protected override char[] PrepareText(string text) {
			int count = text.Length;
			if(CalcCheckSum) {
				text += CalcCCheckDigit(text);
				if(count >= 10)
					text += CalcKCheckDigit(text);
			}
			return ('*' + text + '*').ToCharArray();
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
