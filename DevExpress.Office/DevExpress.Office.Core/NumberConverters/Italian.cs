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
	#region CardinalItalianNumericsProvider
	public class CardinalItalianNumericsProvider : INumericsProvider {
		static string[] separator = { "", "", " ", " e " };
		static string[] generalSingles = { "uno", "due", "tre", "quattro", "cinque", "sei", "sette", "otto", "nove", "zero", "un" };
		static string[] teens = { "dieci", "undici", "dodici", "tredici", "quattordici", "quindici", "sedici", "diciassette", "diciotto", "diciannove" };
		static string[] tenths = { "venti", "trenta", "quaranta", "cinquanta", "sessanta", "settanta", "ottanta", "novanta" };
		static string[] hundreds = { "cento", "duecento", "trecento", "quattrocento", "cinquecento", "seicento", "settecento", "ottocento", "novecento" };
		static string[] thousands = { "mila", "mille" };
		static string[] million = { "milione", "milioni" };
		static string[] billion = { "miliardo", "miliardi" };
		static string[] trillion = { "bilione", "bilioni" };
		static string[] quadrillion = { "biliardo", "biliardi" };
		static string[] quintillion = { "trilione", "trilioni" };
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
	#region CardinalItalianOptionalNumericsProvider
	public class CardinalItalianOptionalNumericsProvider : INumericsProvider {
		static string[] generalSingles = { "uno", "due", "tré", "quattro", "cinque", "sei", "sette", "otto", "nove", "zero" };
		static string[] separator = { "", "", " " };
		static string[] tenths = { "vent", "trent", "quarant", "cinquant", "sessant", "settant", "ottant", "novant" };
		static string[] hundreds = { "cent", "duecent", "trecent", "quattrocent", "cinquecent", "seicent", "settecent", "ottocent", "novecent" };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return generalSingles; } }
		public string[] Singles { get { return generalSingles; } }
		public string[] Teens { get { return null; } }
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
	#region OrdinalItalianNumericsProvider
	public class OrdinalItalianNumericsProvider : INumericsProvider {
		static string[] generalSingles = { "unesimo", "duesimo", "treesimo", "quattresimo", "cinquesimo", "seiesimo", "settesimo", "ottesimo", "novesimo", "zero" };
		static string[] separator = { "", "", " " };
		static string[] teens = { "decimo", "undicesimo", "dodicesimo", "tredicesimo", "quattordicesimo", "quindicesimo", "sedicesimo", "diciassettesimo", "diciottesimo", "diciannovesimo" };
		static string[] tenths = { "ventesimo", "trentesimo", "quarantesimo", "cinquantesimo", "sessantesimo", "settantesimo", "ottantesimo", "novantesimo" };
		static string[] hundreds = { "centesimo", "duecentesimo", "trecentesimo", "quattrocentesimo", "cinquecentesimo", "seicentesimo", "settecentesimo", "ottocentesimo", "novecentesimo" };
		static string[] thousands = { "millesimo", "millesimo" };
		static string[] million = { "milionesimo", "milionesimo" };
		static string[] billion = { "miliardesimo", "miliardesimo" };
		static string[] trillion = { "bilionesimo" };
		static string[] quadrillion = { "biliardesimo" };
		static string[] quintillion = { "trilionesimo" };
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
	#region OrdinalItalianOptionalNumericsProvider
	public class OrdinalItalianOptionalNumericsProvider : INumericsProvider {
		static string[] generalSingles = { "primo", "secondo", "terzo", "quarto", "quinto", "sesto", "settimo", "ottavo", "nono", "zero" };
		static string[] separator = { "", "", " " };
		static string[] teens = { "decimo", "undicesimo", "dodicesimo", "tredicesimo", "quattordicesimo", "quindicesimo", "sedicesimo", "diciassettesimo", "diciottesimo", "diciannovesimo" };
		static string[] tenths = { "ventesimo", "trentesimo", "quarantesimo", "cinquantesimo", "sessantesimo", "settantesimo", "ottantesimo", "novantesimo" };
		static string[] hundreds = { "centesimo", "duecentesimo", "trecentesimo", "quattrocentesimo", "cinquecentesimo", "seicentesimo", "settecentesimo", "ottocentesimo", "novecentesimo" };
		static string[] thousands = { "millesimo", "millesimo" };
		static string[] million = { "milionesimo", "milionesimo" };
		static string[] billion = { "miliardesimo", "miliardesimo" };
		static string[] trillion = { "bilionesimo", "bilionesimo" };
		static string[] quadrillion = { "biliardesimo", "biliardesimo" };
		static string[] quintillion = { "trilionesimo", "trilionesimo" };
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
	#region DescriptiveItalianNumberConverterBase (abstract class)
	public abstract class DescriptiveItalianNumberConverterBase : SomeLanguagesBased {
		bool flagOptionalProvider = false;
		#region Fields
		bool oneHundredValue;
		#endregion
		#region Properties
		protected internal bool OneHundredValue { get { return oneHundredValue; } }
		#endregion
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new CardinalItalianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new CardinalItalianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			if (flagOptionalProvider)
				return new CardinalItalianOptionalNumericsProvider();
			return new CardinalItalianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new CardinalItalianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new CardinalItalianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new CardinalItalianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new CardinalItalianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new CardinalItalianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalItalianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new CardinalItalianNumericsProvider();
		}
		protected internal override void GenerateDigitsInfo(DigitInfoCollection digits, long value) {
			if (value / 1000000000000000000 != 0) {
				FlagIntegerQuintillion = (value % 1000000000000000000 == 0);
				GenerateQuintillionDigits(digits, value / 1000000000000000000);
			}
			value %= 1000000000000000000;
			if (value / 1000000000000000 != 0) {
				FlagIntegerQuadrillion = (value % 1000000000000000 == 0);
				GenerateQuadrillionDigits(digits, value / 1000000000000000);
			}
			value %= 1000000000000000;
			if (value / 1000000000000 != 0) {
				FlagIntegerTrillion = (value % 1000000000000 == 0);
				GenerateTrillionDigits(digits, value / 1000000000000);
			}
			value %= 1000000000000;
			if (value / 1000000000 != 0) {
				FlagIntegerBillion = (value % 1000000000 == 0);
				GenerateBillionDigits(digits, value / 1000000000);
				value %= 1000000000;
			}
			if (value / 1000000 != 0) {
				FlagIntegerMillion = (value % 1000000 == 0);
				GenerateMillionDigits(digits, value / 1000000);
			}
			value %= 1000000;
			if (value / 1000 != 0) {
				FlagIntegerThousand = (value % 1000 == 0);
				GenerateThousandDigits(digits, value / 1000);
			}
			long localValue = value % 1000;
			value %= 1000;
			this.oneHundredValue = localValue % 100 == 0;
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
			if (Type == NumberingFormat.OrdinalText) {
				GenerateDigitsInfo(digits, value);
				digits.Add(new ThousandsDigitInfo(new OrdinalItalianNumericsProvider(), 0));
				FlagThousand = false;
				return;
			}
			if (value == 1) {
				GenerateThousandSeparator(digits, GenerateThousandProvider(), value);
				digits.Add(new ThousandsDigitInfo(GenerateThousandProvider(), value));
			}
			else
				base.GenerateThousandDigits(digits, value);
			FlagThousand = false;
		}
		protected internal override void GenerateTenthsDigits(DigitInfoCollection digits, long value) {
			flagOptionalProvider = ((value % 10 == 1) || (value % 10 == 8));
			base.GenerateTenthsDigits(digits, value);
		}
		protected DigitInfo ChooseCardinalProvider(INumericsProvider provider, int number) {
			if (FlagMillion)
				return new MillionDigitInfo(provider, number);
			if (FlagBillion)
				return new BillionDigitInfo(provider, number);
			if (FlagTrillion)
				return new TrillionDigitInfo(provider, number);
			if (FlagQuadrillion)
				return new QuadrillionDigitInfo(provider, number);
			if (FlagQuintillion)
				return new QuintillionDigitInfo(provider, number);
			return new ThousandsDigitInfo(provider, number);
		}
		protected void GenerateLastDigits(DigitInfoCollection digits, long value) {
			GenerateDigitsInfo(digits, value);
			if (Type == NumberingFormat.CardinalText)
				digits.Add(new SeparatorDigitInfo(GenerateQuintillionProvider(), 2));
			GenerateQuintillionSeparator(digits, GenerateQuintillionProvider(), value);
			if (Type == NumberingFormat.OrdinalText)
				digits.Add(ChooseCardinalProvider(new OrdinalItalianNumericsProvider(), 0));
			else {
				int index = value % 10 == 1 && value % 100 != 11 ? 0 : 1;
				digits.Add(ChooseCardinalProvider(new CardinalItalianNumericsProvider(), index));
			}
		}
	}
	#endregion
	#region DescriptiveCardinalItalianNumberConverter
	public class DescriptiveCardinalItalianNumberConverter : DescriptiveItalianNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
		protected internal override void GenerateDigits(DigitInfoCollection digits, long value) {
			base.GenerateDigits(digits, value);
			if (digits.Count > 1 && digits.Last.Type == DigitType.Single)
				digits.Last.Provider = new CardinalItalianOptionalNumericsProvider();
		}
		protected internal override void GenerateThousandSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0 && IsDigitInfoGreaterThousand(digits.Last))
				digits.Add(new SeparatorDigitInfo(provider, 2));
			else
				base.GenerateTenthsSeparator(digits, provider);
		}
		protected internal override void GenerateHundredSeparator(DigitInfoCollection digits, INumericsProvider provider) {
			if (digits.Count != 0 && OneHundredValue && FlagThousand && !FlagIntegerThousand) {
				digits.Add(new SeparatorDigitInfo(GenerateHundredProvider(), 2));
				return;
			}
			if (digits.Count != 0 && OneHundredValue)
				digits.Add(new SeparatorDigitInfo(GenerateHundredProvider(), 3));
			else {
				if (digits.Count != 0 && IsDigitInfoGreaterThousand(digits.Last))
					digits.Add(new SeparatorDigitInfo(provider, 2));
			}
			base.GenerateHundredSeparator(digits, provider);
		}
		protected internal override void GenerateTenthsSeparator(DigitInfoCollection digits, INumericsProvider provider) {
			if (digits.Count != 0 && IsDigitInfoGreaterThousand(digits.Last))
				digits.Add(new SeparatorDigitInfo(provider, 2));
			else
				base.GenerateTenthsSeparator(digits, provider);
		}
		protected internal override void GenerateTeensSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0 && IsDigitInfoGreaterThousand(digits.Last))
				digits.Add(new SeparatorDigitInfo(provider, 2));
			else
				base.GenerateTeensSeparator(digits, provider, value);
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if (value == 3 && IsValueGreaterThousand && digits.Count != 0) {
				digits.Add(new SingleNumeralDigitInfo(new CardinalItalianOptionalNumericsProvider(), value));
				return;
			}
			if (value != 1 && digits.Count != 0 && IsDigitInfoGreaterThousand(digits.Last)) {
				digits.Add(new SeparatorDigitInfo(GenerateSinglesProvider(), 2));
				digits.Add(new SingleNumeralDigitInfo(GenerateSinglesProvider(), value));
				return;
			}
			if (value == 1 && digits.Count != 0 && (digits.Last.Type == DigitType.Thousand || digits.Last.Type == DigitType.Million)) {
				digits.Add(new SeparatorDigitInfo(GenerateSinglesProvider(), 3));
				digits.Add(new SingleNumeralDigitInfo(GenerateSinglesProvider(), value));
				return;
			}
			if (value == 1 && IsValueGreaterThousand)
				digits.Add(new SingleNumeralDigitInfo(GenerateSinglesProvider(), 11));
			else
				base.GenerateSinglesDigits(digits, value);
		}
	}
	#endregion
	#region DescriptiveOrdinalItalianNumberConverter
	public class DescriptiveOrdinalItalianNumberConverter : DescriptiveItalianNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.OrdinalText; } }
		protected internal override void GenerateDigits(DigitInfoCollection digits, long value) {
			base.GenerateDigits(digits, value);
			if (digits.Count == 1)
				digits.Last.Provider = new OrdinalItalianOptionalNumericsProvider();
			else
				if (value % 1000 > 10)
					digits.Last.Provider = new OrdinalItalianNumericsProvider();
		}
		protected internal override void GenerateHundredSeparator(DigitInfoCollection digits, INumericsProvider provider) {
			if (digits.Count != 0 && IsDigitInfoGreaterHundred(digits.Last))
				digits.Add(new SeparatorDigitInfo(new OrdinalItalianNumericsProvider(), 2));
			else
				base.GenerateHundredSeparator(digits, provider);
		}
		protected internal override void GenerateTenthsDigits(DigitInfoCollection digits, long value) {
			if (digits.Count != 0 && digits.Last.Type == DigitType.Hundred && (value / 10 == 8))
				digits.Last.Provider = new CardinalItalianOptionalNumericsProvider();
			base.GenerateTenthsDigits(digits, value);
		}
		protected internal override void GenerateTenthsSeparator(DigitInfoCollection digits, INumericsProvider provider) {
			if (digits.Count != 0 && IsDigitInfoGreaterHundred(digits.Last))
				digits.Add(new SeparatorDigitInfo(new OrdinalItalianNumericsProvider(), 2));
			else
				base.GenerateTenthsSeparator(digits, provider);
		}
		protected internal override void GenerateTeensSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0 && IsDigitInfoGreaterHundred(digits.Last))
				digits.Add(new SeparatorDigitInfo(new OrdinalItalianNumericsProvider(), 2));
			else
				base.GenerateTeensSeparator(digits, GenerateTeensProvider(), value);
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if (digits.Count != 0 && digits.Last.Type == DigitType.Hundred && (value == 1 || value == 8))
				digits.Last.Provider = new CardinalItalianOptionalNumericsProvider();
			if (value == 1 && IsValueGreaterHundred && digits.Count == 0)
				return;
			if (value == 1 && IsValueGreaterThousand && digits.Count != 0) {
				digits.Add(new SingleDigitInfo(new CardinalItalianNumericsProvider(), 11));
				return;
			}
			if (digits.Count != 0 && value == 1 && IsValueGreaterHundred && IsDigitInfoGreaterHundred(digits.Last)) {
				digits.Add(new SeparatorDigitInfo(new OrdinalItalianNumericsProvider(), 2));
				return;
			}
			if (digits.Count != 0 && value != 1 && IsValueGreaterHundred && IsDigitInfoGreaterHundred(digits.Last)) {
				digits.Add(new SeparatorDigitInfo(new OrdinalItalianNumericsProvider(), 2));
				digits.Add(new SingleDigitInfo(new CardinalItalianNumericsProvider(), value));
				return;
			}
			if (digits.Count != 0 && IsDigitInfoGreaterHundred(digits.Last)) {
				digits.Add(new SeparatorDigitInfo(new OrdinalItalianNumericsProvider(), 2));
				digits.Add(new SingleDigitInfo(new OrdinalItalianOptionalNumericsProvider(), value));
				return;
			}
			base.GenerateSinglesDigits(digits, value);
		}
	}
	#endregion
	#region OrdinalItalianNumberConverter
	public class OrdinalItalianNumberConverter : OrdinalBasedNumberConverter {
		protected internal override NumberingFormat Type { get { return NumberingFormat.Ordinal; } }
		public override string ConvertNumberCore(long value) {
			return String.Format("{0}°", value);
		}
	}
	#endregion
}
