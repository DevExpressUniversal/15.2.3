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
	#region CardinalPortugueseNumericsProvider
	public class CardinalPortugueseNumericsProvider : INumericsProvider {
		static string[] separator = { " ", " e ", "" };
		static string[] singlesNumeral = { "um", "dois", "três", "quatro", "cinco", "seis", "sete", "oito", "nove" };
		static string[] generalSingles = { "um", "dois", "três", "quatro", "cinco", "seis", "sete", "oito", "nove", "zero" };
		static string[] teens = { "dez", "onze", "doze", "treze", "quatorze", "quinze", "dezesseis", "dezessete", "dezoito", "dezenove" };
		static string[] tenths = { "vinte", "trinta", "quarenta", "cinqüenta", "sessenta", "setenta", "oitenta", "noventa" };
		static string[] hundreads = { "cento", "duzentos", "trezentos", "quatrocentos", "quinhentos", "seiscentos", "setecentos", "oitocentos", "novecentos", "cem" };
		static string[] thousands = { "mil" };
		static string[] million = { "milhão", "milhões" };
		static string[] billion = { "mil milhões" };
		static string[] trillion = { "bilião", "biliões" };
		static string[] quadrillion = { "mil biliões" };
		static string[] quintillion = { "trilião", "triliões" };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return singlesNumeral; } }
		public string[] Singles { get { return generalSingles; } }
		public string[] Teens { get { return teens; } }
		public string[] Tenths { get { return tenths; } }
		public string[] Hundreds { get { return hundreads; } }
		public string[] Thousands { get { return thousands; } }
		public string[] Million { get { return million; } }
		public string[] Billion { get { return billion; } }
		public string[] Trillion { get { return trillion; } }
		public string[] Quadrillion { get { return quadrillion; } }
		public string[] Quintillion { get { return quintillion; } }
	}
	#endregion
	#region OrdinalPortugueseNumericsProvider
	public class OrdinalPortugueseNumericsProvider : INumericsProvider {
		static string[] separator = { " ", " ", "" };
		static string[] singlesNumeral = { "", "dois", "três", "quatro", "cinco", "seis", "sete", "oito", "nove", "un" };
		static string[] generalSingles = { "primeiro", "segundo", "terceiro", "quarto", "quinto", "sexto", "sétimo", "oitavo", "nono", "zero" };
		static string[] teens = { "décimo", "décimo primeiro", "décimo segundo", "décimo terceiro", "décimo quarto", "décimo quinto", "décimo sexto", "décimo sétimo", "décimo oitavo", "décimo nono" };
		static string[] tenths = { "vigésimo", "trigésimo", "quadragésimo", "qüinquagésimo", "sexagésimo", "setuagésimo", "octogésimo", "nonagésimo" };
		static string[] hundreds = { "centésimo", "ducentésimo", "trecentésimo", "quadringentésimo", "qüingentésimo", "sexcentésimo", "setingentésimo", "octingentésimo", "nongentésimo" };
		static string[] thousands = { "milésimos", "milésimo" };
		static string[] million = { "milionésimos", "milionésimo" };
		static string[] billion = { "mil milionésimos", "mil milionésimo" };
		static string[] trillion = { "bilionésimos", "bilionésimo" };
		static string[] quadrillion = { "mil bilionésimos", "mil bilionésimo" };
		static string[] quintillion = { "trilionésimos", "trilionésimo" };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return singlesNumeral; } }
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
	#region OrdinalPortugueseOptionalNumericProvider
	public class OrdinalPortugueseOptionalNumericProvider : INumericsProvider {
		static string[] separator = { " ", " e ", "" };
		static string[] singlesNumeral = { "um", "dois", "três", "quatro", "cinco", "seis", "sete", "oito", "nove" };
		static string[] teens = { "dez", "onze", "doze", "treze", "quatorze", "quinze", "dezesseis", "dezessete", "dezoito", "dezenove" };
		static string[] tenths = { "vinte", "trinta", "quarenta", "cinqüenta", "sessenta", "setenta", "oitenta", "noventa" };
		static string[] hundreds = { "cento", "duzentos", "trezentos", "quatrocentos", "quinhentos", "seiscentos", "setecentos", "oitocentos", "novecentos", "cem" };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return singlesNumeral; } }
		public string[] Singles { get { return singlesNumeral; } }
		public string[] Teens { get { return teens; } }
		public string[] Tenths { get { return tenths; } }
		public string[] Hundreds { get { return hundreds; } }
		public string[] Thousands { get { return null; } }
		public string[] Million { get { return null; } }
		public string[] Billion { get { return null; } }
		public string[] Trillion { get { return null; } }
		public string[] Quadrillion { get { return null; } }
		public string[] Quintillion { get { return null; } }
	}
	#endregion
	#region DescriptiveSpanishNumberConverterBase (abstract class)
	public abstract class DescriptivePortugueseNumberConverterBase : DescriptiveNumberConverterBase {
		#region Fields
		bool oneHundredValue;
		bool fullHundreds;
		#endregion
		#region Properties
		protected internal bool OneHundredValue { get { return oneHundredValue; } }
		protected internal bool FullHundreds { get { return fullHundreds; } }
		#endregion
		protected internal override void GenerateDigitsInfo(DigitInfoCollection digits, long value) {
			if (value / 1000000000000000000 != 0) {
				GenerateQuintillionDigits(digits, value / 1000000000000000000);
				FlagQuintillion = false;
			}
			value %= 1000000000000000000;
			if (value / 1000000000000000 != 0) {
				GenerateQuadrillionDigits(digits, value / 1000000000000000);
				FlagQuadrillion = false;
			}
			value %= 1000000000000000;
			if (value / 1000000000000 != 0) {
				GenerateTrillionDigits(digits, value / 1000000000000);
				FlagTrillion = false;
			}
			value %= 1000000000000;
			if (value / 1000000000 != 0) {
				GenerateBillionDigits(digits, value / 1000000000);
				FlagBillion = false;
			}
			value %= 1000000000;
			if (value / 1000000 != 0) {
				GenerateMillionDigits(digits, value / 1000000);
				FlagMillion = false;
			}
			value %= 1000000;
			if (value / 1000 != 0) {
				GenerateThousandDigits(digits, value / 1000);
				FlagThousand = false;
			}
			long localValue = value % 1000;
			if (localValue >= 100)
				this.fullHundreds = localValue % 100 == 0;
			this.oneHundredValue = localValue / 100 == 1 && fullHundreds;
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
		protected internal abstract INumericsProvider GetHundredProvider(INumericsProvider provider);
	}
	#endregion
	#region DescriptiveCardinalPortugueseNumberConverter
	public class DescriptiveCardinalPortugueseNumberConverter : DescriptivePortugueseNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new CardinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new CardinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new CardinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new CardinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new CardinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new CardinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new CardinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new CardinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new CardinalPortugueseNumericsProvider();
		}
		protected internal override void GenerateQuintillionDigits(DigitInfoCollection digits, long value) {
			FlagQuintillion = true;
			GenerateLastDigits(digits, value);
			FlagQuintillion = false;
		}
		protected internal override void GenerateTrillionDigits(DigitInfoCollection digits, long value) {
			FlagTrillion = true;
			GenerateLastDigits(digits, value);
			FlagTrillion = false;
		}
		protected internal override void GenerateMillionDigits(DigitInfoCollection digits, long value) {
			FlagMillion = true;
			GenerateLastDigits(digits, value);
			FlagMillion = false;
		}
		protected internal override void GenerateHundredSeparator(DigitInfoCollection digits, INumericsProvider provider) {
			if (digits.Count != 0 && FullHundreds)
				digits.Add(new SeparatorDigitInfo(provider, 1));
			else
				base.GenerateHundredSeparator(digits, provider);
		}
		protected internal override void GenerateHundredDigits(DigitInfoCollection digits, long value) {
			if (OneHundredValue) {
				GenerateHundredSeparator(digits, GenerateHundredProvider());
				digits.Add(new HundredsDigitInfo(new CardinalPortugueseNumericsProvider(), 10));
			}
			else
				base.GenerateHundredDigits(digits, value);
		}
		protected internal override void GenerateTenthsSeparator(DigitInfoCollection digits, INumericsProvider provider) {
			TeensTenthsSingleSeparator(digits, provider);
		}
		protected internal override void GenerateTeensSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			TeensTenthsSingleSeparator(digits, provider);
		}
		protected internal override void GenerateSinglesSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			TeensTenthsSingleSeparator(digits, provider);
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if ((FlagThousand || FlagBillion || FlagQuadrillion) && value == 1 && digits.Count == 0)
				return;
			base.GenerateSinglesDigits(digits, value);
		}
		protected DigitInfo ChooseCardinalProvider(int number) {
			if (FlagMillion)
				return new MillionDigitInfo(GenerateMillionProvider(), number);
			if (FlagTrillion)
				return new TrillionDigitInfo(GenerateTrillionProvider(), number);
			if (FlagQuintillion)
				return new QuintillionDigitInfo(GenerateQuintillionProvider(), number);
			return new ThousandsDigitInfo(GenerateThousandProvider(), number);
		}
		protected void GenerateLastDigits(DigitInfoCollection digits, long value) {
			GenerateDigitsInfo(digits, value);
			GenerateQuintillionSeparator(digits, GenerateQuintillionProvider(), value);
			if (value > 1)
				digits.Add(ChooseCardinalProvider(1));
			else
				digits.Add(ChooseCardinalProvider(0));
		}
		protected void TeensTenthsSingleSeparator(DigitInfoCollection digits, INumericsProvider provider) {
			if (digits.Count != 0 && IsValueGreaterHundred && IsDigitInfoGreaterThousand(digits.Last)) {
				digits.Add(new SeparatorDigitInfo(provider, 0));
				return;
			}
			if (digits.Count != 0)
				digits.Add(new SeparatorDigitInfo(provider, 1));
		}
		protected internal override INumericsProvider GetHundredProvider(INumericsProvider provider) {
			return provider;
		}
	}
	#endregion
	#region DescriptiveOrdinalPortugueseNumberConverter
	public class DescriptiveOrdinalPortugueseNumberConverter : DescriptivePortugueseNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.OrdinalText; } }
		protected internal override INumericsProvider GenerateSinglesProvider() {
			if (IsValueGreaterHundred)
				return new OrdinalPortugueseOptionalNumericProvider();
			return new OrdinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			if (IsValueGreaterHundred)
				return new OrdinalPortugueseOptionalNumericProvider();
			return new OrdinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new OrdinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new OrdinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new OrdinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new OrdinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new OrdinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new OrdinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new OrdinalPortugueseNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new OrdinalPortugueseNumericsProvider();
		}
		protected internal override void GenerateQuintillionDigits(DigitInfoCollection digits, long value) {
			FlagQuintillion = true;
			GenerateLastDigits(digits, value);
			FlagQuintillion = false;
		}
		protected internal override void GenerateQuadrillionDigits(DigitInfoCollection digits, long value) {
			FlagQuadrillion = true;
			GenerateLastDigits(digits, value);
			FlagQuadrillion = false;
		}
		protected internal override void GenerateTrillionDigits(DigitInfoCollection digits, long value) {
			FlagTrillion = true;
			GenerateLastDigits(digits, value);
			FlagTrillion = false;
		}
		protected internal override void GenerateBillionDigits(DigitInfoCollection digits, long value) {
			FlagBillion = true;
			GenerateLastDigits(digits, value);
			FlagBillion = false;
		}
		protected internal override void GenerateMillionDigits(DigitInfoCollection digits, long value) {
			FlagMillion = true;
			GenerateLastDigits(digits, value);
			FlagMillion = false;
		}
		protected internal override void GenerateThousandDigits(DigitInfoCollection digits, long value) {
			FlagThousand = true;
			GenerateLastDigits(digits, value);
			FlagThousand = false;
		}
		protected internal override void GenerateHundredSeparator(DigitInfoCollection digits, INumericsProvider provider) {
			if (digits.Count != 0 && FullHundreds)
				digits.Add(new SeparatorDigitInfo(new OrdinalPortugueseOptionalNumericProvider(), 1));
			else
				base.GenerateHundredSeparator(digits, provider);
		}
		protected internal override void GenerateHundredDigits(DigitInfoCollection digits, long value) {
			if (IsValueGreaterHundred) {
				if (digits.Count != 0 && FullHundreds)
					digits.Add(new SeparatorDigitInfo(new OrdinalPortugueseOptionalNumericProvider(), 1));
				else
					base.GenerateHundredSeparator(digits, GenerateHundredProvider());
				long index = OneHundredValue ? 10 : value;
				digits.Add(new HundredsDigitInfo(new CardinalPortugueseNumericsProvider(), index));
			}
			else
				base.GenerateHundredDigits(digits, value);
		}
		protected internal override void GenerateTenthsDigits(DigitInfoCollection digits, long value) {
			if (IsValueGreaterHundred) {
				GenerateTenthsSeparator(digits, new OrdinalPortugueseOptionalNumericProvider());
				digits.Add(new TenthsDigitInfo(new OrdinalPortugueseOptionalNumericProvider(), value / 10));
				if (value % 10 != 0) {
					digits.Add(new SeparatorDigitInfo(new OrdinalPortugueseOptionalNumericProvider(), 1));
					digits.Add(new SingleNumeralDigitInfo(new OrdinalPortugueseOptionalNumericProvider(), value % 10));
				}
			}
			else
				base.GenerateTenthsDigits(digits, value);
		}
		protected internal override INumericsProvider GetHundredProvider(INumericsProvider provider) {
			return new OrdinalPortugueseOptionalNumericProvider();
		}
		protected DigitInfo ChooseOrdinalProvider(int number) {
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
			if (value == 1) {
				digits.Add(ChooseOrdinalProvider(1));
				return;
			}
			if (value % 100 != 1) {
				GenerateDigitsInfo(digits, value);
				digits.Add(new SeparatorDigitInfo(new OrdinalPortugueseNumericsProvider(), 0));
				digits.Add(ChooseOrdinalProvider(0));
				return;
			}
			long index = value / 100 == 1 ? value % 100 : value / 100;
			digits.Add(new HundredsDigitInfo(new CardinalPortugueseNumericsProvider(), index));
			digits.Add(new SeparatorDigitInfo(new OrdinalPortugueseNumericsProvider(), 0));
			digits.Add(ChooseOrdinalProvider(1));
		}
	}
	#endregion
	#region OrdinalPortugueseNumberConverter
	public class OrdinalPortugueseNumberConverter : OrdinalBasedNumberConverter {
		protected internal override NumberingFormat Type { get { return NumberingFormat.Ordinal; } }
		public override string ConvertNumberCore(long value) {
			return String.Format("{0}º", value);
		}
	}
	#endregion
}
