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
	#region CardinalFrenchNumericsProvider
	public class CardinalFrenchNumericsProvider : INumericsProvider {
		internal static string[] separator = { " ", "-", " et ", "" };
		static string[] generalSingles = { "un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf", "zéro", "s" };
		internal static string[] teens = { "dix", "onze", "douze", "treize", "quatorze", "quinze", "seize", "dix-sept", "dix-huit", "dix-neuf" };
		internal static string[] tenths = { "vingt", "trente", "quarante", "cinquante", "soixante", "soixante", "quatre-vingt", "quatre-vingt" };
		static string[] hundreds = { "cent", "deux cent", "trois cent", "quatre cent", "cinq cent", "six cent", "sept cent", "huit cent", "neuf cent" };
		internal static string[] thousands = { "mille" };
		internal static string[] million = { "million", "millions" };
		internal static string[] billion = { "milliard", "milliards" };
		internal static string[] trillion = { "billion", "billions" };
		internal static string[] quadrillion = { "billiard", "billiards" };
		internal static string[] quintillion = { "trillion", "trillions" };
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
	#region CardinalFrenchOptionalNumericsProvider
	public class CardinalFrenchOptionalNumericsProvider : INumericsProvider {
		static string[] generalSingles = { "un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf", "zéro" };
		static string[] hundreds = { "cent", "deux cents", "trois cents", "quatre cents", "cinq cents", "six cents", "sept cents", "huit cents", "neuf cents" };
		public string[] Separator { get { return CardinalFrenchNumericsProvider.separator; } }
		public string[] SinglesNumeral { get { return generalSingles; } }
		public string[] Singles { get { return generalSingles; } }
		public string[] Teens { get { return CardinalFrenchNumericsProvider.teens; } }
		public string[] Tenths { get { return CardinalFrenchNumericsProvider.tenths; } }
		public string[] Hundreds { get { return hundreds; } }
		public string[] Thousands { get { return CardinalFrenchNumericsProvider.thousands; } }
		public string[] Million { get { return CardinalFrenchNumericsProvider.million; } }
		public string[] Billion { get { return CardinalFrenchNumericsProvider.billion; } }
		public string[] Trillion { get { return CardinalFrenchNumericsProvider.trillion; } }
		public string[] Quadrillion { get { return CardinalFrenchNumericsProvider.quadrillion; } }
		public string[] Quintillion { get { return CardinalFrenchNumericsProvider.quintillion; } }
	}
	#endregion
	#region OrdinalFrenchNumericsProvider
	public class OrdinalFrenchNumericsProvider : INumericsProvider {
		static string[] generalSingles = { "unième", "deuxième", "troisième", "quatrième", "cinquième", "sixième", "septième", "huitième", "neuvième", "zéro", "ième" };
		internal static string[] separator = { " ", "-", " et ", "" };
		internal static string[] teens = { "dixième", "onzième", "douzième", "treizième", "quatorzième", "quinzième", "seizième", "dix-septième", "dix-huitième", "dix-neuvième" };
		internal static string[] tenths = { "vingtième", "trentième", "quarantième", "cinquantième", "soixantième", "soixante", "quatre-vingts", "quatre-vingt-dix" };
		internal static string[] hundreds = { "centième", "deux centième", "trois centième", "quatre centième", "cinq centième", "six centième", "sept centième", "huit centième", "neuf centième" };
		internal static string[] thousands = { "millième" };
		internal static string[] million = { "millionième", "millionième" };
		internal static string[] billion = { "milliardième", "milliardième" };
		internal static string[] trillion = { "billionième", "billionième" };
		internal static string[] quadrillion = { "billiardième", "billiardième" };
		internal static string[] quintillion = { "trillionième", "trillionième" };
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
	#region OrdinalFrenchOptionalNumericsProvider
	public class OrdinalFrenchOptionalNumericsProvider : INumericsProvider {
		static string[] generalSingles = { "premier", "deuxième", "troisième", "quatrième", "cinquième", "sixième", "septième", "huitième", "neuvième", "zéro", "ième" };
		public string[] Separator { get { return OrdinalFrenchNumericsProvider.separator; } }
		public string[] SinglesNumeral { get { return generalSingles; } }
		public string[] Singles { get { return generalSingles; } }
		public string[] Teens { get { return OrdinalFrenchNumericsProvider.teens; } }
		public string[] Tenths { get { return OrdinalFrenchNumericsProvider.tenths; } }
		public string[] Hundreds { get { return OrdinalFrenchNumericsProvider.hundreds; } }
		public string[] Thousands { get { return OrdinalFrenchNumericsProvider.thousands; } }
		public string[] Million { get { return OrdinalFrenchNumericsProvider.million; } }
		public string[] Billion { get { return OrdinalFrenchNumericsProvider.billion; } }
		public string[] Trillion { get { return OrdinalFrenchNumericsProvider.trillion; } }
		public string[] Quadrillion { get { return OrdinalFrenchNumericsProvider.quadrillion; } }
		public string[] Quintillion { get { return OrdinalFrenchNumericsProvider.quintillion; } }
	}
	#endregion
	#region DescriptiveFrenchNumberConverterBase (abstract class)
	public abstract class DescriptiveFrenchNumberConverterBase : DescriptiveNumberConverterBase {
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new CardinalFrenchNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new CardinalFrenchNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new CardinalFrenchNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new CardinalFrenchNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new CardinalFrenchNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new CardinalFrenchNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new CardinalFrenchNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new CardinalFrenchNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalFrenchNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new CardinalFrenchNumericsProvider();
		}
		protected internal override void GenerateQuintillionDigits(DigitInfoCollection digits, long value) {
			FlagQuintillion = true;
			GenerateLastDigit(digits, value);
			FlagQuintillion = false;
		}
		protected internal override void GenerateQuadrillionDigits(DigitInfoCollection digits, long value) {
			FlagQuadrillion = true;
			GenerateLastDigit(digits, value);
			FlagQuadrillion = false;
		}
		protected internal override void GenerateTrillionDigits(DigitInfoCollection digits, long value) {
			FlagTrillion = true;
			GenerateLastDigit(digits, value);
			FlagTrillion = false;
		}
		protected internal override void GenerateBillionDigits(DigitInfoCollection digits, long value) {
			FlagBillion = true;
			GenerateLastDigit(digits, value);
			FlagBillion = false;
		}
		protected internal override void GenerateMillionDigits(DigitInfoCollection digits, long value) {
			FlagMillion = true;
			GenerateLastDigit(digits, value);
			FlagMillion = false;
		}
		protected internal override void GenerateThousandDigits(DigitInfoCollection digits, long value) {
			if (value == 1) {
				GenerateThousandSeparator(digits, GenerateThousandProvider(), value);
				digits.Add(new ThousandsDigitInfo(GenerateThousandProvider(), 0));
			}
			else
				base.GenerateThousandDigits(digits, value);
		}
		protected internal override void GenerateTeensSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0) {
				if (digits.Last.Type == DigitType.Tenth) {
					if ((digits.Last.Number == 5) && (value == 1))
						digits.Add(new SeparatorDigitInfo(provider, 2));
					else
						digits.Add(new SeparatorDigitInfo(provider, 1));
				}
				else
					digits.Add(new SeparatorDigitInfo(provider, 0));
			}
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if (digits.Count != 0 && digits.Last.Type == DigitType.Tenth) {
				long lastDigit = digits.Last.Number;
				if (lastDigit == 5 || lastDigit == 7) {
					GenerateTeensDigits(digits, value);
					return;
				}
				if (lastDigit == 6) {
					if (value == 0) {
						digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), 11));
						return;
					}
					GenerateSinglesSeparator(digits, GenerateSinglesProvider(), value);
					digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value));
					return;
				}
			}
			base.GenerateSinglesDigits(digits, value);
		}
		protected internal override void GenerateSinglesSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0) {
				if (digits.Last.Type == DigitType.Tenth) {
					if (value == 1 && digits.Last.Number != 6)
						digits.Add(new SeparatorDigitInfo(provider, 2));
					else
						digits.Add(new SeparatorDigitInfo(provider, 1));
				}
				else
					digits.Add(new SeparatorDigitInfo(provider, 0));
			}
		}
		protected void GenerateLastDigit(DigitInfoCollection digits, long value) {
			base.GenerateDigits(digits, value);
			digits.Add(new SeparatorDigitInfo(GenerateMillionProvider(), 0));
			if (value % 100 == 1)
				digits.Add(ChooseCardinalProvider(0));
			else
				digits.Add(ChooseCardinalProvider(1));
		}
		protected DigitInfo ChooseCardinalProvider(long value) {
			if (FlagMillion)
				return new MillionDigitInfo(GenerateMillionProvider(), value);
			if (FlagBillion)
				return new BillionDigitInfo(GenerateBillionProvider(), value);
			if (FlagTrillion)
				return new TrillionDigitInfo(GenerateTrillionProvider(), value);
			if (FlagQuadrillion)
				return new QuadrillionDigitInfo(GenerateQuadrillionProvider(), value);
			if (FlagQuintillion)
				return new QuintillionDigitInfo(GenerateQuintillionProvider(), value);
			return new ThousandsDigitInfo(GenerateThousandProvider(), value);
		}
	}
	#endregion
	#region DescriptiveCardinalFrenchNumberConverter
	public class DescriptiveCardinalFrenchNumberConverter : DescriptiveFrenchNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
		protected internal override void GenerateDigits(DigitInfoCollection digits, long value) {
			base.GenerateDigits(digits, value);
			if (digits.Count == 1)
				digits.Last.Provider = new CardinalFrenchOptionalNumericsProvider();
		}
	}
	#endregion
	#region DescriptiveOrdinalFrenchNumberConverter
	public class DescriptiveOrdinalFrenchNumberConverter : DescriptiveFrenchNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.OrdinalText; } }
		protected internal override void GenerateDigits(DigitInfoCollection digits, long value) {
			base.GenerateDigits(digits, value);
			if (digits.Count == 1)
				digits.Last.Provider = new OrdinalFrenchOptionalNumericsProvider();
			else
				digits.Last.Provider = new OrdinalFrenchNumericsProvider();
		}
	}
	#endregion
	#region OrdinalFrenchNumberConverter
	public class OrdinalFrenchNumberConverter : OrdinalBasedNumberConverter {
		protected internal override NumberingFormat Type { get { return NumberingFormat.Ordinal; } }
		public override string ConvertNumberCore(long value) {
			if (value == 1)
				return String.Format("{0}er", value);
			else
				return String.Format("{0}e", value);
		}
	}
	#endregion
}
