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
	#region SlavicBaseCardinal (abstract class)
	public abstract class SlavicOrdinalBase : SlavicBase {
		protected internal override void GenerateDigits(DigitInfoCollection digits, long value) {
			base.GenerateDigits(digits, value);
			if (FlagIntegerThousand)
				GenerateDigitsCore(digits, CheckTypeIntegerThousand);
			else if (FlagIntegerMillion)
				GenerateDigitsCore(digits, CheckTypeIntegerMillion);
			else if (FlagIntegerBillion)
				GenerateDigitsCore(digits, CheckTypeIntegerBillion);
			else if (FlagIntegerTrillion)
				GenerateDigitsCore(digits, CheckTypeIntegerTrillion);
			else if (FlagIntegerQuadrillion)
				GenerateDigitsCore(digits, CheckTypeIntegerQuadrillion);
			else if (FlagIntegerQuintillion)
				for (int i = 0; i <= digits.Count - 1; i++)
					digits[i].Provider = GetProvider();
			digits.Last.Provider = GetOrdinalSlavicNumericsProvider();
		}
		void GenerateDigitsCore(DigitInfoCollection digits, CheckTypeDelegate checkType) {
			int start = digits.Count - 1;
			for (int i = start; i >= 0; i--) {
				if (i == 0) {
					digits[i].Provider = GetProvider();
					continue;
				}
				DigitType type = digits[i - 1].Type;
				if (checkType(type))
					break;
				digits[i].Provider = GetProvider();
			}
		}
		bool CheckTypeIntegerThousand(DigitType type) {
			return type == DigitType.Million || type == DigitType.Billion ||
				type == DigitType.Trillion || type == DigitType.Quadrillion || type == DigitType.Quintillion;
		}
		bool CheckTypeIntegerMillion(DigitType type) {
			return type == DigitType.Billion || type == DigitType.Trillion ||
				type == DigitType.Quadrillion || type == DigitType.Quintillion;
		}
		bool CheckTypeIntegerBillion(DigitType type) {
			return type == DigitType.Trillion || type == DigitType.Quadrillion || type == DigitType.Quintillion;
		}
		bool CheckTypeIntegerTrillion(DigitType type) {
			return type == DigitType.Quadrillion || type == DigitType.Quintillion;
		}
		bool CheckTypeIntegerQuadrillion(DigitType type) {
			return type == DigitType.Quintillion;
		}
		protected internal override void GenerateQuintillionDigits(DigitInfoCollection digits, long value) {
			if (!FlagIntegerQuintillion)
				base.GenerateQuintillionDigits(digits, value);
			else
				GenerateIntegerLastDigit(digits, value);
		}
		protected internal override void GenerateQuadrillionDigits(DigitInfoCollection digits, long value) {
			if (!FlagIntegerQuadrillion)
				base.GenerateQuadrillionDigits(digits, value);
			else
				GenerateIntegerLastDigit(digits, value);
		}
		protected internal override void GenerateTrillionDigits(DigitInfoCollection digits, long value) {
			if (!FlagIntegerTrillion)
				base.GenerateTrillionDigits(digits, value);
			else
				GenerateIntegerLastDigit(digits, value);
		}
		protected internal override void GenerateBillionDigits(DigitInfoCollection digits, long value) {
			if (!FlagIntegerBillion)
				base.GenerateBillionDigits(digits, value);
			else
				GenerateIntegerLastDigit(digits, value);
		}
		protected internal override void GenerateMillionDigits(DigitInfoCollection digits, long value) {
			if (!FlagIntegerMillion)
				base.GenerateMillionDigits(digits, value);
			else
				GenerateIntegerLastDigit(digits, value);
		}
		protected internal override void GenerateThousandDigits(DigitInfoCollection digits, long value) {
			if (!FlagIntegerThousand)
				base.GenerateThousandDigits(digits, value);
			else
				GenerateIntegerLastDigit(digits, value);
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if (digits.Count == 0 && value == 1 && (FlagIntegerThousand || FlagIntegerMillion || FlagIntegerBillion || FlagIntegerTrillion || FlagIntegerQuadrillion || FlagIntegerQuintillion)) {
				return;
			}
			base.GenerateSinglesDigits(digits, value);
		}
		protected void GenerateIntegerLastDigit(DigitInfoCollection digits, long value) {
			if (value == 1) {
				digits.Add(new SeparatorDigitInfo(new OrdinalRussianNumericsProvider(), 0));
				digits.Add(ChooseOrdinalProvider());
			}
			else {
				GenerateDigitsInfo(digits, value);
				digits.Add(ChooseOrdinalProvider());
			}
		}
		public DigitInfo ChooseOrdinalProvider() {
			if (FlagIntegerMillion)
				return new MillionDigitInfo(GenerateMillionProvider(), 0);
			if (FlagIntegerBillion)
				return new BillionDigitInfo(GenerateBillionProvider(), 0);
			if (FlagIntegerTrillion)
				return new TrillionDigitInfo(GenerateTrillionProvider(), 0);
			if (FlagIntegerQuadrillion)
				return new QuadrillionDigitInfo(GenerateQuadrillionProvider(), 0);
			if (FlagIntegerQuintillion)
				return new QuintillionDigitInfo(GenerateQuintillionProvider(), 0);
			return new ThousandsDigitInfo(GenerateThousandProvider(), 0);
		}
		protected internal abstract INumericsProvider GetProvider();
		protected internal abstract INumericsProvider GetOrdinalSlavicNumericsProvider();
		delegate bool CheckTypeDelegate(DigitType type);
	}
}
	#endregion
