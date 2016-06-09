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
using System.Collections.Generic;
using DevExpress.Office.Utils;
using System.Text;
namespace DevExpress.Office.NumberConverters {
	#region CardinalThaiNumericsProvider
	public class CardinalThaiNumericsProvider : INumericsProvider {
		internal static string[] separator = { "", "", };
		internal static string[] generalSingles = { "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า", "ศูนย์", "เอ็ด" };
		internal static string[] teens = { "สิบ", "สิบเอ็ด", "สิบสอง", "สิบสาม", "สิบสี่", "สิบห้า", "สิบหก", "สิบเจ็ด", "สิบแปด", "สิบเก้า" };
		internal static string[] tenths = { "ยี่สิบ", "สามสิบ", "สี่สิบ", "ห้าสิบ", "หกสิบ", "เจ็ดสิบ", "แปดสิบ", "เก้าสิบ" };
		internal static string[] hundreds = { "หนึ่งร้อย", "สองร้อย", "สามร้อย", "สี่ร้อย", "ห้าร้อย", "หกร้อย", "เจ็ดร้อย", "แปดร้อย", "เก้าร้อย" };
		static string[] thousands = { "พัน" };
		internal static string[] million = { "ล้าน" };
		internal static string[] billion = { "พันล้าน" };
		internal static string[] trillion = { "ล้านล้าน" };
		internal static string[] quadrillion = { "พันล้านล้าน" };
		internal static string[] quintillion = { "ล้านล้านล้าน" };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return generalSingles; } }
		public string[] Singles { get { return generalSingles; } }
		public string[] Teens { get { return teens; } }
		public string[] Tenths { get { return tenths; } }
		public string[] Hundreds { get { return hundreds; } }
		public string[] Thousands { get { return thousands; } }
		public string[] Million { get { return million; } }
		public string[] Billion { get { return billion; } }
		public string[] Trillion { get { return trillion; } }
		public string[] Quadrillion { get { return quadrillion; } }
		public string[] Quintillion { get { return quintillion; } }
	}
	#endregion
	#region CardinalThaiOptionalNumericProvider
	public class CardinalThaiOptionalNumericsProvider : INumericsProvider {
		static string[] thousands = { "หมื่น", "แสน", "พัน" };
		public string[] Separator { get { return null; } }
		public string[] SinglesNumeral { get { return null; } }
		public string[] Singles { get { return null; } }
		public string[] Teens { get { return null; } }
		public string[] Tenths { get { return null; } }
		public string[] Hundreds { get { return null; } }
		public string[] Thousands { get { return thousands; } }
		public string[] Million { get { return null; } }
		public string[] Billion { get { return null; } }
		public string[] Trillion { get { return null; } }
		public string[] Quadrillion { get { return null; } }
		public string[] Quintillion { get { return null; } }
	}
	#endregion
	#region DescriptiveCardinalThaiNumberConverter
	public class DescriptiveCardinalThaiNumberConverter : DescriptiveThaiNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
	}
	#endregion
	#region DescriptiveOrdinalThaiNumberConverter
	public class DescriptiveOrdinalThaiNumberConverter : DescriptiveThaiNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.OrdinalText; } }
		protected internal override string ConvertDigitsToString(DigitInfoCollection digits) {
			StringBuilder result = new StringBuilder();
			int count = digits.Count;
			result.Append("ที่");
			for (int i = 0; i < count; i++)
				result.Append(digits[i].ConvertToString());
			if (result.Length > 0)
				result[0] = Char.ToUpper(result[0]);
			return result.ToString();
		}
	}
	#endregion
	#region DescriptiveThaiNumberConverterBase (abstract class)
	public abstract class DescriptiveThaiNumberConverterBase : DescriptiveNumberConverterBase {
		#region Fields
		protected internal const long maxThaiValue = 10000000000000000;
		#endregion
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new CardinalThaiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new CardinalThaiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new CardinalThaiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new CardinalThaiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new CardinalThaiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new CardinalThaiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new CardinalThaiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new CardinalThaiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalThaiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new CardinalThaiNumericsProvider();
		}
		protected internal override void GenerateDigitsInfo(DigitInfoCollection digits, long value) {
			if (value / 1000000000000000000 != 0)
				GenerateQuintillionDigits(digits, value / 1000000);
			value %= 1000000000000000000;
			if (value / 1000000000000000 != 0)
				GenerateQuadrillionDigits(digits, value / 1000000);
			value %= 1000000000000000;
			if (value / 1000000000000 != 0)
				GenerateTrillionDigits(digits, value / 1000000);
			value %= 1000000000000;
			if (value / 1000000000 != 0)
				GenerateBillionDigits(digits, value / 1000000);
			value %= 1000000000;
			if (value / 1000000 != 0)
				GenerateMillionDigits(digits, value / 1000000);
			value %= 1000000;
			if (value / 1000 != 0)
				GenerateThousandDigits(digits, value / 1000);
			value %= 1000;
			if (value / 100 != 0)
				GenerateHundredDigits(digits, value / 100);
			value %= 100;
			if (value == 0)
				return;
			if (value >= 20)
				GenerateTenthsDigits(digits, value);
			else {
				if (value >= 10)
					GenerateTeensDigits(digits, value % 10);
				else
					GenerateSinglesDigits(digits, value);
			}
		}
		protected internal override void GenerateQuintillionDigits(DigitInfoCollection digits, long value) {
			if (value % 1000000000000 == 0)
				base.GenerateQuintillionDigits(digits, value / 1000000000000);
			else {
				GenerateSinglesDigits(digits, value / 1000000000000);
				if ((value / 1000000) % 1000 != 0 || (value / 1000000000) % 1000 != 0) 
					digits.Add(new MillionDigitInfo(GenerateMillionProvider(), 0));
				else if (value % 1000000 != 0) 
					digits.Add(new TrillionDigitInfo(GenerateTrillionProvider(), 0));
			}
		}
		protected internal override void GenerateQuadrillionDigits(DigitInfoCollection digits, long value) {
			FlagQuadrillion = true;
			if (value >= 100000000000) {
				GenerateHundredDigits(digits, value / 100000000000);
				if (value % 100000000000 == 0)
					digits.Add(new TrillionDigitInfo(GenerateTrillionProvider(), 0));
				value %= 100000000000;
			}
			if (value >= 10000000000 && value < 100000000000) {
				GenerateTenthsDigits(digits, value / 1000000000);
				if (value % 10000000000 == 0)
					digits.Add(new TrillionDigitInfo(GenerateQuadrillionProvider(), 0));
				else if (value % 1000000000 == 0)
					digits.Add(new QuadrillionDigitInfo(GenerateQuadrillionProvider(), 0));
				else if ((value / 1000000000) % 10 == 0) {
					if ((value / 1000000) % 1000 != 0) {
						FlagQuadrillion = false;
						return;
					}
					digits.Add(new MillionDigitInfo(GenerateMillionProvider(), 0));
				}
				else if ((value / 1000) % 1000000 == 0 || (value / 1000000) % 1000 == 0)
					digits.Add(new BillionDigitInfo(GenerateBillionProvider(), 0));
				else
					digits.Add(new ThousandsDigitInfo(new CardinalThaiOptionalNumericsProvider(), 2));
			}
			if (value >= 1000000000 && value < 10000000000) {
				if (value % 1000000000 == 0)
					base.GenerateQuadrillionDigits(digits, value / 1000000000);
				else {
					GenerateSinglesDigits(digits, value / 1000000000);
					digits.Add(new ThousandsDigitInfo(new CardinalThaiOptionalNumericsProvider(), 2));
					if (value % 1000000000 != 0 && (value % 1000000000) / 1000000 == 0)
						digits.Add(new MillionDigitInfo(GenerateMillionProvider(), 0));
				}
			}
			FlagQuadrillion = false;
		}
		protected internal override void GenerateTrillionDigits(DigitInfoCollection digits, long value) {
			FlagTrillion = true;
			if (value % 1000000 == 0)
				base.GenerateTrillionDigits(digits, value / 1000000);
			else {
				if (value >= 100000000) {
					GenerateHundredDigits(digits, value / 100000000);
					value %= 100000000;
				}
				if (value > 20000000 && value < 100000000)
					GenerateTenthsDigits(digits, value / 1000000);
				if (value >= 10000000 && value < 20000000)
					digits.Add(new TeensDigitInfo(GenerateTeensProvider(), (value % 10000000) / 1000000));
				if (value < 10000000)
					GenerateSinglesDigits(digits, value / 1000000);
				digits.Add(new MillionDigitInfo(GenerateMillionProvider(), 0));
			}
			FlagTrillion = false;
		}
		protected internal override void GenerateBillionDigits(DigitInfoCollection digits, long value) {
			FlagBillion = true;
			if (value >= 100000) {
				GenerateHundredDigits(digits, value / 100000);
				if (value % 100000 == 0)
					digits.Add(new MillionDigitInfo(GenerateMillionProvider(), 0));
				value %= 100000;
			}
			if (value >= 10000 && value < 100000) {
				GenerateTenthsDigits(digits, value / 1000);
				if ((value / 1000) % 10 != 0)
					digits.Add(new ThousandsDigitInfo(new CardinalThaiOptionalNumericsProvider(), 2));
				if (value % 1000 == 0 || value % 10000 == 0)
					digits.Add(new MillionDigitInfo(GenerateMillionProvider(), 0));
			}
			if (value >= 1000 && value < 10000) {
				if (value % 1000 == 0)
					base.GenerateBillionDigits(digits, value / 1000);
				else {
					GenerateSinglesDigits(digits, value / 1000);
					digits.Add(new ThousandsDigitInfo(new CardinalThaiOptionalNumericsProvider(), 2));
				}
			}
			FlagBillion = false;
		}
		protected internal override void GenerateThousandDigits(DigitInfoCollection digits, long value) {
			FlagThousand = true;
			if (value >= 100 && value % 10 == 0) {
				GenerateHundredDigits(digits, value / 100);
				if (value % 100 != 0)
					GenerateThousandDigits(digits, value % 100);
			}
			else if (value < 100 && value % 10 == 0)
				GenerateTenthsDigits(digits, value % 100);
			else
				base.GenerateThousandDigits(digits, value);
			FlagThousand = false;
		}
		protected internal override void GenerateHundredDigits(DigitInfoCollection digits, long value) {
			if (FlagThousand || FlagBillion || FlagQuadrillion) {
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value));
				digits.Add(new ThousandsDigitInfo(new CardinalThaiOptionalNumericsProvider(), 1));
			}
			else
				base.GenerateHundredDigits(digits, value);
		}
		protected internal override void GenerateTenthsDigits(DigitInfoCollection digits, long value) {
			if (FlagThousand || FlagBillion || FlagQuadrillion) {
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value / 10));
				digits.Add(new ThousandsDigitInfo(new CardinalThaiOptionalNumericsProvider(), 0));
				if (value % 10 != 0)
					digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value % 10));
			}
			else
				base.GenerateTenthsDigits(digits, value);
		}
		protected internal override void GenerateTeensDigits(DigitInfoCollection digits, long value) {
			if (FlagThousand || FlagBillion) {
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), 1));
				digits.Add(new ThousandsDigitInfo(new CardinalThaiOptionalNumericsProvider(), 0));
				if (value % 10 != 0)
					digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value % 10));
			}
			else
				base.GenerateTeensDigits(digits, value);
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if (digits.Count != 0 && value == 1 && !FlagThousand)
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), 11));
			else
				base.GenerateSinglesDigits(digits, value);
		}
	}
	#endregion
}
