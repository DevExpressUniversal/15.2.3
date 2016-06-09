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
using DevExpress.XtraPrinting.BarCode.Native;
using System.ComponentModel;
namespace DevExpress.XtraPrinting.BarCode {
	public class Matrix2of5Generator : Industrial2of5Generator {
		static Hashtable charPattern = new Hashtable();
		#region static XRMatrix2of5Generator()
		static Matrix2of5Generator() {
			charPattern['0'] = "nnwwnn";
			charPattern['1'] = "wnnnwn";
			charPattern['2'] = "nwnnwn";
			charPattern['3'] = "wwnnnn";
			charPattern['4'] = "nnwnwn";
			charPattern['5'] = "wnwnnn";
			charPattern['6'] = "nwwnnn";
			charPattern['7'] = "nnnwwn";
			charPattern['8'] = "wnnwnn";
			charPattern['9'] = "nwnwnn";
			charPattern['B'] = "wnnnnn";
		}
		#endregion
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("Matrix2of5GeneratorSymbologyCode")]
#endif
		public override BarCodeSymbology SymbologyCode {
			get {
				return BarCodeSymbology.Matrix2of5;
			}
		}
		public Matrix2of5Generator() {
		}
		Matrix2of5Generator(Matrix2of5Generator source)
			: base(source) {
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new Matrix2of5Generator(this);
		}
		protected override Hashtable GetPatternTable() {
			return charPattern;
		}
		protected override string FormatText(string text) {
			if(CalcCheckSum)
				text += CalcCheckDigit(text);
			return text;
		}
		protected override char[] PrepareText(string text) {
			return ('B' + text + 'B').ToCharArray();
		}
	}
}
