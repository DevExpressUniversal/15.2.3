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
namespace DevExpress.Office.NumberConverters {
	#region CardinalGermanNumericsProvider
	public class CardinalGermanNumericsProvider : INumericsProvider {
		static string[] separator = { "", "", "und", " " };
		static string[] generalSingles = { "ein", "zwei", "drei", "vier", "fünf", "sechs", "sieben", "acht", "neun", "null", "eine" };
		internal static string[] teens = { "zehn", "elf", "zwölf", "dreizehn", "vierzehn", "fünfzehn", "sechzehn", "siebzehn", "achtzehn", "neunzehn" };
		internal static string[] tenths = { "zwanzig", "dreißig", "vierzig", "fünfzig", "sechzig", "siebzig", "achtzig", "neunzig" };
		static string[] hundreds = { "hundert", "zweihundert", "dreihundert", "vierhundert", "fünfhundert", "sechshundert", "siebenhundert", "achthundert", "neunhundert" };
		internal static string[] thousands = { "tausend" };
		internal static string[] million = { "million", "millionen" };
		internal static string[] billion = { "milliarde", "milliarden" };
		internal static string[] trillion = { "billion", "billionen" };
		internal static string[] quadrillion = { "billiarde", "billiarden" };
		internal static string[] quintillion = { "trillion", "trillionen" };
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
	#region CardinalGermanOptional_1_NumericsProvider
	public class CardinalGermanOptional_1_NumericsProvider : INumericsProvider {
		internal static string[] generalSingles = { "eins", "zwei", "drei", "vier", "fünf", "sechs", "sieben", "acht", "neun", "null" };
		static string[] separator = { "", "", "und" };
		static string[] hundreds = { "hundert", "zweihundert", "dreihundert", "vierhundert", "fünfhundert", "sechshundert", "siebenhundert", "achthundert", "neunhundert" };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return generalSingles; } }
		public string[] Singles { get { return generalSingles; } }
		public string[] Teens { get { return CardinalGermanNumericsProvider.teens; } }
		public string[] Tenths { get { return CardinalGermanNumericsProvider.tenths; } }
		public string[] Hundreds { get { return hundreds; } }
		public string[] Thousands { get { return CardinalGermanNumericsProvider.thousands; } }
		public string[] Million { get { return CardinalGermanNumericsProvider.million; } }
		public string[] Billion { get { return CardinalGermanNumericsProvider.billion; } }
		public string[] Trillion { get { return CardinalGermanNumericsProvider.trillion; } }
		public string[] Quadrillion { get { return CardinalGermanNumericsProvider.quadrillion; } }
		public string[] Quintillion { get { return CardinalGermanNumericsProvider.quintillion; } }
	}
	#endregion
	#region CardinalGermanOptional_2_NumericsProvider
	public class CardinalGermanOptional_2_NumericsProvider : INumericsProvider {
		static string[] separator = { "", "", "und" };
		static string[] hundreds = { "einhundert", "zweihundert", "dreihundert", "vierhundert", "fünfhundert", "sechshundert", "siebenhundert", "achthundert", "neunhundert" };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return CardinalGermanOptional_1_NumericsProvider.generalSingles; } }
		public string[] Singles { get { return CardinalGermanOptional_1_NumericsProvider.generalSingles; } }
		public string[] Teens { get { return CardinalGermanNumericsProvider.teens; } }
		public string[] Tenths { get { return CardinalGermanNumericsProvider.tenths; } }
		public string[] Hundreds { get { return hundreds; } }
		public string[] Thousands { get { return CardinalGermanNumericsProvider.thousands; } }
		public string[] Million { get { return CardinalGermanNumericsProvider.million; } }
		public string[] Billion { get { return CardinalGermanNumericsProvider.billion; } }
		public string[] Trillion { get { return CardinalGermanNumericsProvider.trillion; } }
		public string[] Quadrillion { get { return CardinalGermanNumericsProvider.quadrillion; } }
		public string[] Quintillion { get { return CardinalGermanNumericsProvider.quintillion; } }
	}
	#endregion
	#region OrdinalGermanNumericsProvider
	public class OrdinalGermanNumericsProvider : INumericsProvider {
		internal static string[] generalSingles = { "erste", "zweite", "dritte", "vierte", "fünfte", "sechste", "siebente", "achte", "neunte", "nullte" };
		internal static string[] separator = { "", "", "und" };
		internal static string[] teens = { "zehnte", "elfte", "zwölfte", "dreizehnte", "vierzehnte", "fünfzehnte", "sechzehnte", "siebzehnte", "achtzehnte", "neunzehnte" };
		internal static string[] tenths = { "zwanzigste", "dreißigste", "vierzigste", "fünfzigste", "sechzigste", "siebzigste", "achtzigste", "neunzigste" };
		static string[] hundreds = { "hundertste", "zweihundertste", "dreihundertste", "vierhundertste", "fünfhundertste", "sechshundertste", "siebenhundertste", "achthundertste", "neunhundertste" };
		internal static string[] thousands = { "tausendste" };
		internal static string[] million = { "millionste", "millionste" };
		internal static string[] billion = { "milliardeste", "milliardeste" };
		internal static string[] trillion = { "billionste", "milliardeste" };
		internal static string[] quadrillion = { "billiardeste", "billiardeste" };
		internal static string[] quintillion = { "trillionste", "trillionste" };
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
	#region OrdinalGermanOptionalNumericsProivder
	public class OrdinalGermanOptionalNumericsProvider : INumericsProvider {
		static string[] hundreds = { "einhundertste", "zweihundertste", "dreihundertste", "vierhundertste", "fünfhundertste", "sechshundertste", "siebenhundertste", "achthundertste", "neunhundertste" };
		public string[] Separator { get { return OrdinalGermanNumericsProvider.separator; } }
		public string[] SinglesNumeral { get { return OrdinalGermanNumericsProvider.generalSingles; } }
		public string[] Singles { get { return OrdinalGermanNumericsProvider.generalSingles; } }
		public string[] Teens { get { return OrdinalGermanNumericsProvider.teens; } }
		public string[] Tenths { get { return OrdinalGermanNumericsProvider.tenths; } }
		public string[] Hundreds { get { return hundreds; } }
		public string[] Thousands { get { return OrdinalGermanNumericsProvider.thousands; } }
		public string[] Million { get { return OrdinalGermanNumericsProvider.million; } }
		public string[] Billion { get { return OrdinalGermanNumericsProvider.billion; } }
		public string[] Trillion { get { return OrdinalGermanNumericsProvider.trillion; } }
		public string[] Quadrillion { get { return OrdinalGermanNumericsProvider.quadrillion; } }
		public string[] Quintillion { get { return OrdinalGermanNumericsProvider.quintillion; } }
	}
	#endregion
	#region DescriptiveGermanNumberConverterBase (abstract class)
	public abstract class DescriptiveGermanNumberConverterBase : SomeLanguagesBased {
		#region Properties
		protected internal bool FlagGermanHundred { get; set; }
		protected internal bool FlagGermanTenths { get; set; }
		#endregion
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new CardinalGermanNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new CardinalGermanNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new CardinalGermanNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			if (FlagGermanHundred)
				return new CardinalGermanOptional_2_NumericsProvider();
			else
				return new CardinalGermanNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new CardinalGermanNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new CardinalGermanNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new CardinalGermanNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new CardinalGermanNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalGermanNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new CardinalGermanNumericsProvider();
		}
		protected internal override void GenerateQuintillionDigits(DigitInfoCollection digits, long value) {
			FlagQuintillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerQuintillion)
				digits.Add(new SeparatorDigitInfo(GenerateQuintillionProvider(), 3));
			FlagQuintillion = false;
		}
		protected internal override void GenerateQuadrillionDigits(DigitInfoCollection digits, long value) {
			FlagQuadrillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerQuadrillion)
				digits.Add(new SeparatorDigitInfo(GenerateQuadrillionProvider(), 3));
			FlagQuadrillion = false;
		}
		protected internal override void GenerateTrillionDigits(DigitInfoCollection digits, long value) {
			FlagTrillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerTrillion)
				digits.Add(new SeparatorDigitInfo(GenerateTrillionProvider(), 3));
			FlagTrillion = false;
		}
		protected internal override void GenerateBillionDigits(DigitInfoCollection digits, long value) {
			FlagBillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerBillion)
				digits.Add(new SeparatorDigitInfo(GenerateBillionProvider(), 3));
			FlagBillion = false;
		}
		protected internal override void GenerateMillionDigits(DigitInfoCollection digits, long value) {
			FlagMillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerMillion)
				digits.Add(new SeparatorDigitInfo(GenerateMillionProvider(), 3));
			FlagMillion = false;
		}
		protected internal override void GenerateHundredDigits(DigitInfoCollection digits, long value) {
			FlagGermanHundred = false;
			if (digits.Count != 0 && digits.Last.Type == DigitType.Thousand)
				FlagGermanHundred = true;
			base.GenerateHundredDigits(digits, value);
		}
		protected internal override void GenerateTenthsDigits(DigitInfoCollection digits, long value) {
			FlagGermanTenths = (value % 100 > 20);
			if (value % 10 == 0)
				FlagGermanTenths = false;
			else
				GenerateSinglesDigits(digits, value % 10);
			GenerateTenthsSeparator(digits, GenerateTenthsProvider());
			digits.Add(new TenthsDigitInfo(GenerateTenthsProvider(), value / 10));
		}
		protected internal override void GenerateTenthsSeparator(DigitInfoCollection digits, INumericsProvider provider) {
			if (digits.Count == 0)
				return;
			DigitType type = digits.Last.Type;
			if (type == DigitType.Single || type == DigitType.SingleNumeral)
				digits.Add(new SeparatorDigitInfo(provider, 2));
			base.GenerateTenthsSeparator(digits, provider);
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if (IsValueGreaterThousand) {
				long index = (!FlagGermanTenths && value == 1) ? 11 : value % 10;
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), index));
			}
			else
				base.GenerateSinglesDigits(digits, value);
			FlagGermanTenths = false;
		}
		protected DigitInfo GetCardinalProvider(int number) {
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
			GenerateDigitsInfo(digits, value);
			digits.Add(new SeparatorDigitInfo(GenerateQuintillionProvider(), 3));
			if (value % 100 == 1)
				digits.Add(GetCardinalProvider(0));
			else
				digits.Add(GetCardinalProvider(1));
		}
	}
	#endregion
	#region DescriptiveCardinalGermanNumberConverter
	public class DescriptiveCardinalGermanNumberConverter : DescriptiveGermanNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
		protected internal override void GenerateDigits(DigitInfoCollection digits, long value) {
			base.GenerateDigits(digits, value);
			if (digits.Count != 0 && digits.Last.Type != DigitType.Hundred)
				digits.Last.Provider = new CardinalGermanOptional_1_NumericsProvider();
		}
	}
	#endregion
	#region DescriptiveOrdinalGermanNumberConverter
	public class DescriptiveOrdinalGermanNumberConverter : DescriptiveGermanNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
		protected internal override void GenerateDigits(DigitInfoCollection digits, long value) {
			base.GenerateDigits(digits, value);
			if (!FlagGermanHundred)
				digits.Last.Provider = new OrdinalGermanNumericsProvider();
			else
				digits.Last.Provider = new OrdinalGermanOptionalNumericsProvider();
		}
	}
	#endregion
	#region OrdinalGermanNumberConverter
	public class OrdinalGermanNumberConverter : OrdinalBasedNumberConverter {
		protected internal override NumberingFormat Type { get { return NumberingFormat.Ordinal; } }
		public override string ConvertNumberCore(long value) {
			return String.Format("{0}.", value);
		}
	}
	#endregion
}
