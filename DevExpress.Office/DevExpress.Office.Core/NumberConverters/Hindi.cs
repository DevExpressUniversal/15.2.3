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
	#region CardinalHindiNumericsProvider
	public class CardinalHindiNumericsProvider : INumericsProvider {
		static string[] separator = { " " };
		static string[] singleNumeral = { "शून्य" };
		static string[] generalSingles = { "एक", "दो", "तीन", "चार", "पांच", "छः", "सात", "आठ", "नौ", "दस", "ग्यारह", "बारह", "तेरह", "चौदह", "पंद्रह", "सोलह", "सत्रह", "अट्ठारह", "उन्नीस", "बीस",
											 "इक्कीस", "बाईस", "तेईस", "चौबीस", "पच्चीस", "छब्बीस", "सत्ताईस", "अट्ठाईस", "उनतीस", "तीस", "इकतीस", "बत्तीस", "तैंतीस", "चौंतीस", "पैंतीस", "छत्तीस",
											 "सैंतीस", "अड़तीस", "उनतालीस", "चालीस", "इकतालीस", "बयालीस", "तैंतालीस", "चौवालीस", "पैंतालीस", "छियालीस", "सैंतालीस", "अड़तालीस", "उनचास", "पचास",
											 "इक्यावन", "बावन", "तिरेपन", "चौवन", "पचपन", "छप्पन", "सत्तावन", "अट्ठावन", "उनसठ", "साठ", "इकसठ", "बासठ", "तिरेसठ", "चौंसठ", "पैंसठ", "छियासठ",
											 "सड़सठ", "अड़सठ", "उनहत्तर", "सत्तर", "इकहत्तर", "बहत्तर", "तिहत्तर", "चौहत्तर", "पचहत्तर", "छिहत्तर", "सतहत्तर", "अठहत्तर", "उनासी", "अस्सी", "इक्यासी",
											 "बयासी", "तिरासी", "चौरासी", "पचासी", "छियासी", "सतासी", "अठासी", "नवासी", "नब्बे", "इक्यानवे", "बानवे", "तिरानवे", "चौरानवे", "पचानवे", "छियानवे", "सत्तानवे", 
											 "अट्ठानवे", "निन्यानवे" };
		static string[] hundreads = {  "सौ",  "लाख" };
		static string[] thousands = {  "हज़ार" };
		static string[] million = {  "करोड़" };
		static string[] billion = {  "अरब",  "खरब" };
		static string[] trillion = {  "नील" };
		static string[] quadrillion = {  "पद्म"  , "शंख" };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return singleNumeral; } }
		public string[] Singles { get { return generalSingles; } }
		public string[] Teens { get { return null; } }
		public string[] Tenths { get { return null; } }
		public string[] Hundreds { get { return hundreads; } }
		public string[] Thousands { get { return thousands; } }
		public string[] Million { get { return million; } }
		public string[] Billion { get { return billion; } }
		public string[] Trillion { get { return trillion; } }
		public string[] Quadrillion { get { return quadrillion; } }
		public string[] Quintillion { get { return null; } }
	}
	#endregion
	#region OrdinalHindiNumericsProvider
	public class OrdinalHindiNumericsProvider : INumericsProvider {
		static string[] separator = { " " };
		static string[] singleNumeral = { "शून्यवाँ" };
		static string[] generalSingles = { "पहला", "दूसरा", "तीसरा", "चौथा", "पांचवाँ", "छठा", "सातवाँ", "आठवाँ", "नौवाँ", "दसवाँ", "ग्यारहवाँ", "बारहवाँ", "तेरहवाँ", "चौदहवाँ", "पंद्रहवाँ", "सोलहवाँ", "सत्रहवाँ", "अट्ठारहवाँ", "उन्नीसवाँ", "बीसवाँ",
											 "इक्कीसवाँ", "बाईसवाँ", "तेईसवाँ", "चौबीसवाँ", "पच्चीसवाँ", "छब्बीसवाँ", "सत्ताईसवाँ", "अट्ठाईसवाँ", "उनतीसवाँ", "तीसवाँ", "इकतीसवाँ", "बत्तीसवाँ", "तैंतीसवाँ", "चौंतीसवाँ", "पैंतीसवाँ", "छत्तीसवाँ",
											 "सैंतीसवाँ", "अड़तीसवाँ", "उनतालीसवाँ", "चालीसवाँ", "इकतालीसवाँ", "बयालीसवाँ", "तैंतालीसवाँ", "चौवालीसवाँ", "पैंतालीसवाँ", "छियालीसवाँ", "सैंतालीसवाँ", "अड़तालीसवाँ", "उनचासवाँ", "पचासवाँ",
											 "इक्यावनवाँ", "बावनवाँ", "तिरेपनवाँ", "चौवनवाँ", "पचपनवाँ", "छप्पनवाँ", "सत्तावनवाँ", "अट्ठावनवाँ", "उनसठवाँ", "साठवाँ", "इकसठवाँ", "बासठवाँ", "तिरेसठवाँ", "चौंसठवाँ", "पैंसठवाँ", "छियासठवाँ",
											 "सड़सठवाँ", "अड़सठवाँ", "उनहत्तरवाँ", "सत्तरवाँ", "इकहत्तरवाँ", "बहत्तरवाँ", "तिहत्तरवाँ", "चौहत्तरवाँ", "पचहत्तरवाँ", "छिहत्तरवाँ", "सतहत्तरवाँ", "अठहत्तरवाँ", "उनासीवाँ", "अस्सीवाँ", "इक्यासीवाँ",
											 "बयासीवाँ", "तिरासीवाँ", "चौरासीवाँ", "पचासीवाँ", "छियासीवाँ", "सतासीवाँ", "अठासीवाँ", "नवासीवाँ", "नब्बेवाँ", "इक्यानवेवाँ", "बानवेवाँ", "तिरानवेवाँ", "चौरानवेवाँ", "पचानवेवाँ", "छियानवेवाँ", "सत्तानवेवाँ", 
											 "अट्ठानवेवाँ", "निन्यानवेवाँ" };
		static string[] hundreads = { "सौवाँ", "लाखवाँ" };
		static string[] thousands = { "हज़ारवाँ" };
		static string[] million = { "करोड़वाँ" };
		static string[] billion = { "अरबवाँ", "खरबवाँ" };
		static string[] trillion = { "नीलवाँ" };
		static string[] quadrillion = { "पद्मवाँ", "शंखवाँ" };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return singleNumeral; } }
		public string[] Singles { get { return generalSingles; } }
		public string[] Teens { get { return null; } }
		public string[] Tenths { get { return null; } }
		public string[] Hundreds { get { return hundreads; } }
		public string[] Thousands { get { return thousands; } }
		public string[] Million { get { return million; } }
		public string[] Billion { get { return billion; } }
		public string[] Trillion { get { return trillion; } }
		public string[] Quadrillion { get { return quadrillion; } }
		public string[] Quintillion { get { return null; } }
	}
	#endregion
	#region OrdinalHindiOptionalNumericsProvider
	public class OrdinalHindiOptionalNumericsProvider : INumericsProvider {
		static string[] generalSingles = { "एकवाँ", "दोवाँ", "तीनवाँ", "चारवाँ", "पांचवाँ", "छठवाँ", "सातवाँ", "आठवाँ", "नौवाँ", "दसवाँ", "ग्यारहवाँ", "बारहवाँ", "तेरहवाँ", "चौदहवाँ", "पंद्रहवाँ", "सोलहवाँ", "सत्रहवाँ", "अट्ठारहवाँ", "उन्नीसवाँ", "बीसवाँ",
											 "इक्कीसवाँ", "बाईसवाँ", "तेईसवाँ", "चौबीसवाँ", "पच्चीसवाँ", "छब्बीसवाँ", "सत्ताईसवाँ", "अट्ठाईसवाँ", "उनतीसवाँ", "तीसवाँ", "इकतीसवाँ", "बत्तीसवाँ", "तैंतीसवाँ", "चौंतीसवाँ", "पैंतीसवाँ", "छत्तीसवाँ",
											 "सैंतीसवाँ", "अड़तीसवाँ", "उनतालीसवाँ", "चालीसवाँ", "इकतालीसवाँ", "बयालीसवाँ", "तैंतालीसवाँ", "चौवालीसवाँ", "पैंतालीसवाँ", "छियालीसवाँ", "सैंतालीसवाँ", "अड़तालीसवाँ", "उनचासवाँ", "पचासवाँ",
											 "इक्यावनवाँ", "बावनवाँ", "तिरेपनवाँ", "चौवनवाँ", "पचपनवाँ", "छप्पनवाँ", "सत्तावनवाँ", "अट्ठावनवाँ", "उनसठवाँ", "साठवाँ", "इकसठवाँ", "बासठवाँ", "तिरेसठवाँ", "चौंसठवाँ", "पैंसठवाँ", "छियासठवाँ",
											 "सड़सठवाँ", "अड़सठवाँ", "उनहत्तरवाँ", "सत्तरवाँ", "इकहत्तरवाँ", "बहत्तरवाँ", "तिहत्तरवाँ", "चौहत्तरवाँ", "पचहत्तरवाँ", "छिहत्तरवाँ", "सतहत्तरवाँ", "अठहत्तरवाँ", "उनासीवाँ", "अस्सीवाँ", "इक्यासीवाँ",
											 "बयासीवाँ", "तिरासीवाँ", "चौरासीवाँ", "पचासीवाँ", "छियासीवाँ", "सतासीवाँ", "अठासीवाँ", "नवासीवाँ", "नब्बेवाँ", "इक्यानवेवाँ", "बानवेवाँ", "तिरानवेवाँ", "चौरानवेवाँ", "पचानवेवाँ", "छियानवेवाँ", "सत्तानवेवाँ", 
											 "अट्ठानवेवाँ", "निन्यानवेवाँ" };
		public string[] Separator { get { return null; } }
		public string[] SinglesNumeral { get { return null; } }
		public string[] Singles { get { return generalSingles; } }
		public string[] Teens { get { return null; } }
		public string[] Tenths { get { return null; } }
		public string[] Hundreds { get { return null; } }
		public string[] Thousands { get { return null; } }
		public string[] Million { get { return null; } }
		public string[] Billion { get { return null; } }
		public string[] Trillion { get { return null; } }
		public string[] Quadrillion { get { return null; } }
		public string[] Quintillion { get { return null; } }
	}
	#endregion
	#region DescriptiveHindiNumberConverterBase (abstract class)
	public abstract class DescriptiveHindiNumberConverterBase : DescriptiveNumberConverterBase {
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new CardinalHindiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new CardinalHindiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new CardinalHindiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new CardinalHindiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new CardinalHindiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new CardinalHindiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new CardinalHindiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new CardinalHindiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalHindiNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new CardinalHindiNumericsProvider();
		}
		protected internal override void GenerateDigitsInfo(DigitInfoCollection digits, long value) {
			if (value / 1000000000000000000 != 0)
				GenerateQuintillionDigits(digits, value);
			if (value / 1000000000000000 != 0)
				GenerateQuadrillionDigits(digits, value);
			if (value / 1000000000000 != 0)
				GenerateTrillionDigits(digits, value);
			if (value / 1000000000 != 0)
				GenerateBillionDigits(digits, value);
			if (value / 1000000 != 0)
				GenerateMillionDigits(digits, value);
			if (value / 1000 != 0)
				GenerateThousandDigits(digits, value);
			value %= 1000;
			if (value / 100 != 0) {
				GenerateHundredDigits(digits, value / 100);
			}
			value %= 100;
			if (value == 0)
				return;
			if (value > 0)
				GenerateSinglesDigits(digits, value);
		}
		protected internal override void GenerateQuintillionDigits(DigitInfoCollection digits, long value) {
			return;
		}
		protected internal override void GenerateQuadrillionDigits(DigitInfoCollection digits, long value) {
			if (value >= 100000000000000000) {
				if (digits.Count != 0)
					GenerateSinglesSeparator(digits, GenerateSinglesProvider(), 0);
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value / 100000000000000000));
				GenerateSinglesSeparator(digits, GenerateSinglesProvider(), 0);
				digits.Add(new QuadrillionDigitInfo(GenerateQuadrillionProvider(), 1));
				value %= 100000000000000000;
				if (value % 100000000000000000 == 0)
					return;
			}
			if (value >= 1000000000000000 && value < 100000000000000000) {
				if (digits.Count != 0)
					GenerateSinglesSeparator(digits, GenerateSinglesProvider(), 0);
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value / 1000000000000000));
				GenerateThousandSeparator(digits, GenerateThousandProvider(), value);
				digits.Add(new QuadrillionDigitInfo(GenerateQuadrillionProvider(), 0));
			}
		}
		protected internal override void GenerateTrillionDigits(DigitInfoCollection digits, long value) {
			if (digits.Count != 0) {
				if (value % 1000000000000000 == 0)
					return;
				value %= 1000000000000000;
			}
			if (value < 10000000000000)
				return;
			else {
				if (digits.Count != 0)
					GenerateSinglesSeparator(digits, GenerateSinglesProvider(), 0);
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value / 10000000000000));
				GenerateSinglesSeparator(digits, GenerateSinglesProvider(), 0);
				digits.Add(new TrillionDigitInfo(GenerateTrillionProvider(), 0));
			}
		}
		protected internal override void GenerateBillionDigits(DigitInfoCollection digits, long value) {
			if (digits.Count != 0) {
				if (value % 10000000 == 0)
					return;
				value %= 10000000000000;
			}
			if (value >= 100000000000) {
				if (digits.Count != 0)
					GenerateSinglesSeparator(digits, GenerateSinglesProvider(), 0);
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value / 100000000000));
				GenerateSinglesSeparator(digits, GenerateSinglesProvider(), 0);
				digits.Add(new BillionDigitInfo(GenerateBillionProvider(), 1));
				value %= 100000000000;
				if (value % 100000000000 == 0)
					return;
			}
			if (value >= 1000000000 && value < 100000000000) {
				if (digits.Count != 0)
					GenerateSinglesSeparator(digits, GenerateSinglesProvider(), 0);
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value / 1000000000));
				GenerateThousandSeparator(digits, GenerateThousandProvider(), value);
				digits.Add(new BillionDigitInfo(GenerateBillionProvider(), 0));
			}
		}
		protected internal override void GenerateMillionDigits(DigitInfoCollection digits, long value) {
			if (digits.Count != 0) {
				if (value % 1000000000 == 0)
					return;
				value %= 1000000000;
			}
			if (value < 10000000)
				return;
			else {
				if (digits.Count != 0)
					GenerateSinglesSeparator(digits, GenerateSinglesProvider(), 0);
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value / 10000000));
				GenerateSinglesSeparator(digits, GenerateSinglesProvider(), 0);
				digits.Add(new MillionDigitInfo(GenerateMillionProvider(), 0));
			}
		}
		protected internal override void GenerateThousandDigits(DigitInfoCollection digits, long value) {
			if (digits.Count != 0) {
				if (value % 10000000 == 0)
					return;
				value %= 10000000;
			}
			if (value >= 100000) {
				if (digits.Count != 0)
					GenerateSinglesSeparator(digits, GenerateSinglesProvider(), 0);
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value / 100000));
				GenerateSinglesSeparator(digits, GenerateSinglesProvider(), 0);
				digits.Add(new HundredsDigitInfo(GenerateHundredProvider(), 2));
				value = value % 100000;
				if (value % 100000 == 0)
					return;
			}
			if (value >= 1000 && value < 100000) {
				if (digits.Count != 0)
					GenerateSinglesSeparator(digits, GenerateSinglesProvider(), 0);
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value / 1000));
				GenerateThousandSeparator(digits, GenerateThousandProvider(), value);
				digits.Add(new ThousandsDigitInfo(GenerateThousandProvider(), 0));
			}
		}
		protected internal override void GenerateHundredDigits(DigitInfoCollection digits, long value) {
			if (digits.Count != 0)
				digits.Add(new SeparatorDigitInfo(GenerateSinglesProvider(), 0));
			if (value < 10)
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value));
			GenerateHundredSeparator(digits, GenerateHundredProvider());
			digits.Add(new HundredsDigitInfo(GenerateHundredProvider(), 1));
		}
		protected internal override void AddZero(DigitInfoCollection digits) {
			digits.Add(new SingleNumeralDigitInfo(GenerateSinglesProvider(), 1));
		}
	}
	#endregion
	#region DescriptiveCardinalHindiNumberConverter
	public class DescriptiveCardinalHindiNumberConverter : DescriptiveHindiNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
	}
	#endregion
	#region DescriptiveOrdinalHindiNumberConverter
	public class DescriptiveOrdinalHindiNumberConverter : DescriptiveHindiNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.OrdinalText; } }
		protected internal override void GenerateDigits(DigitInfoCollection digits, long value) {
			base.GenerateDigits(digits, value);
			if (digits.Last.GetType() == typeof(SingleDigitInfo) && digits.Count > 1) {
				digits.Last.Provider = new OrdinalHindiOptionalNumericsProvider();
				return;
			}
			digits.Last.Provider = new OrdinalHindiNumericsProvider();
		}
	}
	#endregion
	#region OrdinalHindiNumberConverter
	public class OrdinalHindiNumberConverter : DescriptiveOrdinalHindiNumberConverter {
		protected internal override NumberingFormat Type { get { return NumberingFormat.Ordinal; } }
	}
	#endregion
}
