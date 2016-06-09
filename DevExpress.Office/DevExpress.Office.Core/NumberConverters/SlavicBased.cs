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
namespace DevExpress.Office.NumberConverters {
	#region SlavicBase (abstract class)
	public abstract class SlavicBase : SomeLanguagesBased {
		protected internal override void GenerateQuintillionDigits(DigitInfoCollection digits, long value) {
			FlagQuintillion = true;
			GenerateDigitsInfo(digits, value);
			GenerateQuintillionSeparator(digits, GenerateQuintillionProvider(), value);
			GenerateLastDigits(digits, value);
			FlagQuintillion = false;
		}
		protected internal override void GenerateQuadrillionDigits(DigitInfoCollection digits, long value) {
			FlagQuadrillion = true;
			GenerateDigitsInfo(digits, value);
			GenerateQuadrillionSeparator(digits, GenerateQuadrillionProvider(), value);
			GenerateLastDigits(digits, value);
			FlagQuadrillion = false;
		}
		protected internal override void GenerateTrillionDigits(DigitInfoCollection digits, long value) {
			FlagTrillion = true;
			GenerateDigitsInfo(digits, value);
			GenerateTrillionSeparator(digits, GenerateTrillionProvider(), value);
			GenerateLastDigits(digits, value);
			FlagTrillion = false;
		}
		protected internal override void GenerateBillionDigits(DigitInfoCollection digits, long value) {
			FlagBillion = true;
			GenerateDigitsInfo(digits, value);
			GenerateBillionSeparator(digits, GenerateBillionProvider(), value);
			GenerateLastDigits(digits, value);
			FlagBillion = false;
		}
		protected internal override void GenerateMillionDigits(DigitInfoCollection digits, long value) {
			FlagMillion = true;
			GenerateDigitsInfo(digits, value);
			GenerateMillionSeparator(digits, GenerateMillionProvider(), value);
			GenerateLastDigits(digits, value);
			FlagMillion = false;
		}
		protected internal override void GenerateThousandDigits(DigitInfoCollection digits, long value) {
			FlagThousand = true;
			GenerateDigitsInfo(digits, value);
			GenerateThousandSeparator(digits, GenerateThousandProvider(), value);
			GenerateLastDigits(digits, value);
			FlagThousand = false;
		}
		protected DigitInfo ChooseCardinalProvider(int number) {
			if (FlagMillion)
				return new MillionDigitInfo(GenerateMillionProvider(), number);
			if (FlagBillion)
				return new BillionDigitInfo(GenerateBillionProvider(), number);
			if (FlagTrillion)
				return new TrillionDigitInfo(GenerateTrillionProvider(), number);
			if (FlagQuadrillion)
				return new QuadrillionDigitInfo(GenerateQuadrillionProvider(), number);
			if (FlagQuintillion)
				return new QuintillionDigitInfo(GenerateQuintillionProvider(), number);
			return new ThousandsDigitInfo(GenerateThousandProvider(), number);
		}
		protected void GenerateLastDigits(DigitInfoCollection digits, long value) {
			long temp = value % 100;
			if (temp > 9 && temp < 21)
				digits.Add(ChooseCardinalProvider(2));
			else {
				temp %= 10;
				if (temp == 1)
					digits.Add(ChooseCardinalProvider(0));
				if (temp > 1 && temp < 5)
					digits.Add(ChooseCardinalProvider(1));
				if (temp > 4 && temp <= 9 || temp == 0)
					digits.Add(ChooseCardinalProvider(2));
			}
		}
	}
	#endregion
}
