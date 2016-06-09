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
namespace DevExpress.Office.NumberConverters {
	#region CardinalSpanishNumericsProvider
	public class CardinalSpanishNumericsProvider : INumericsProvider {
		static string[] separator = { " ", " y ", "" };
		static string[] singlesNumeral = { "un", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve" };
		static string[] generalSingles = { "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve", "cero" };
		static string[] teens = { "diez", "once", "doce", "trece", "catorce", "quince", "dieciséis", "diecisiete", "dieciocho", "diecinueve" };
		static string[] tenths = { "veinte", "treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa" };
		static string[] hundreds = { "ciento", "doscientos", "trescientos", "cuatrocientos", "quinientos", "seiscientos", "setecientos", "ochocientos", "novecientos", "cien" };
		static string[] thousands = { "mil" };
		static string[] million = { "millón", "millones" };
		static string[] billion = { "millardo", "millardos" };
		static string[] trillion = { "billón", "billónes" };
		static string[] quadrillion = { "billardo", "billardos" };
		static string[] quintillion = { "trillón", "trillón" };
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
	#region CardinalSpanishOptionalNumericsProvider
	public class CardinalSpanishOptionalNumericsProvider : INumericsProvider {
		internal static string[] tenths = { "veintiuno", "veintidós", "veintitrés", "veinticuatro", "veinticinco", "veintiséis", "veintisiete", "veintiocho", "veintinueve" };
		public string[] Separator { get { return null; } }
		public string[] SinglesNumeral { get { return null; } }
		public string[] Singles { get { return null; } }
		public string[] Teens { get { return null; } }
		public string[] Tenths { get { return tenths; } }
		public string[] Hundreds { get { return null; } }
		public string[] Thousands { get { return null; } }
		public string[] Million { get { return null; } }
		public string[] Billion { get { return null; } }
		public string[] Trillion { get { return null; } }
		public string[] Quadrillion { get { return null; } }
		public string[] Quintillion { get { return null; } }
	}
	#endregion
	#region OrdinalSpanishNumericsProvider
	public class OrdinalSpanishNumericsProvider : INumericsProvider {
		static string[] separator = { " ", " ", "" };
		static string[] singlesNumeral = { "", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve", "un" };
		static string[] generalSingles = { "primero", "segundo", "tercero", "cuarto", "quinto", "sexto", "séptimo", "octavo", "noveno", "cero" };
		static string[] teens = { "décimo", "undécimo", "duodécimo", "decimotercero", "decimocuarto", "decimoquinto", "decimosexto", "decimoséptimo", "decimoctavo", "decimonoveno" };
		static string[] tenths = { "vigésimo", "trigésimo", "cuadragésimo", "quincuagésimo", "sexagésimo", "septuagésimo", "octogésimo", "nonagésimo" };
		static string[] hundreds = { "centésimo", "ducentésimo", "tricentésimo", "cuadringentésimo", "quingentésimo", "sexcentésimo", "septingentésimo", "octingentésimo", "noningentésimo" };
		static string[] thousands = { "milésimo" };
		static string[] million = { "millonésimo" };
		static string[] billion = { "millardésimo" };
		static string[] trillion = { "billonésimo" };
		static string[] quadrillion = { "billardésimo" };
		static string[] quintillion = { "trillónésimo" };
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
	#region OrdinalSpanishOptionalNumericsProvider
	public class OrdinalSpanishOptionalNumericsProvider : INumericsProvider {
		static string[] separator = { " ", " y ", "" };
		static string[] singlesNumeral = { "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve" };
		static string[] teens = { "diez", "once", "doce", "trece", "catorce", "quince", "dieciséis", "diecisiete", "dieciocho", "diecinueve" };
		static string[] hundreds = { "ciento", "doscientos", "trescientos", "cuatrocientos", "quincientos", "seiscientos", "sietecientos", "ochocientos", "nuevecientos", "cien" };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return singlesNumeral; } }
		public string[] Singles { get { return null; } }
		public string[] Teens { get { return teens; } }
		public string[] Tenths { get { return null; } }
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
	public abstract class DescriptiveSpanishNumberConverterBase : DescriptiveNumberConverterBase {
		#region Fileds
		bool oneHundredValue;
		bool hundredOneValue;
		#endregion
		#region Properties
		protected internal bool OneHundredValue { get { return oneHundredValue; } }
		protected internal bool HundredOneValue { get { return hundredOneValue; } }
		#endregion
		protected internal override void GenerateThousandSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (value == 1)
				digits.Add(new SeparatorDigitInfo(provider, 2));
			else
				base.GenerateThousandSeparator(digits, provider, value);
		}
		protected internal override void GenerateDigitsInfo(DigitInfoCollection digits, long value) {
			long localValue = value % 1000;
			this.oneHundredValue = localValue / 100 == 1 && localValue % 100 == 0;
			this.hundredOneValue = localValue / 100 == 1 && localValue % 100 == 1;
			base.GenerateDigitsInfo(digits, value);
		}
	}
	#endregion
	#region DescriptiveCardinalSpanishNumberConverter
	public class DescriptiveCardinalSpanishNumberConverter : DescriptiveSpanishNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new CardinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new CardinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new CardinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new CardinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new CardinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new CardinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new CardinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new CardinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new CardinalSpanishNumericsProvider();
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
		protected internal override void GenerateHundredDigits(DigitInfoCollection digits, long value) {
			if (value == 1 && IsValueGreaterHundred && OneHundredValue) {
				if (digits.Count != 0)
					digits.Add(new SeparatorDigitInfo(new CardinalSpanishNumericsProvider(), 0));
				digits.Add(new HundredsDigitInfo(new CardinalSpanishNumericsProvider(), 10));
			}
			else
				base.GenerateHundredDigits(digits, value);
		}
		protected internal override void GenerateTenthsDigits(DigitInfoCollection digits, long value) {
			if (value > 20 && value < 30) {
				GenerateTenthsSeparator(digits, GenerateTenthsProvider());
				digits.Add(new TenthsDigitInfo(new CardinalSpanishOptionalNumericsProvider(), value % 10 + 1));
			}
			else
				base.GenerateTenthsDigits(digits, value);
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if (FlagThousand && value == 1 && digits.Count == 0)
				return;
			if (digits.Count != 0 && value == 1 && FlagThousand && IsDigitInfoGreaterThousand(digits.Last)) {
				digits.Add(new SeparatorDigitInfo(GenerateSinglesProvider(), 0));
				return;
			}
			if (digits.Count == 0 && IsValueGreaterThousand)
				digits.Add(new SingleNumeralDigitInfo(GenerateSinglesProvider(), value));
			else
				base.GenerateSinglesDigits(digits, value);
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
	#region DescriptiveOrdinalSpanishNumberConverter
	public class DescriptiveOrdinalSpanishNumberConverter : DescriptiveSpanishNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.OrdinalText; } }
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new OrdinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new OrdinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new OrdinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new OrdinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new OrdinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new OrdinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new OrdinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new OrdinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new OrdinalSpanishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new OrdinalSpanishNumericsProvider();
		}
		protected internal override void GenerateQuintillionDigits(DigitInfoCollection digits, long value) {
			FlagQuintillion = true;
			GenerateDigitsInfo(digits, value);
			digits.Add(new QuintillionDigitInfo(GenerateQuintillionProvider(), 0));
			FlagQuintillion = false;
		}
		protected internal override void GenerateQuadrillionDigits(DigitInfoCollection digits, long value) {
			FlagQuadrillion = true;
			GenerateDigitsInfo(digits, value);
			digits.Add(new QuadrillionDigitInfo(GenerateQuadrillionProvider(), 0));
			FlagQuadrillion = false;
		}
		protected internal override void GenerateTrillionDigits(DigitInfoCollection digits, long value) {
			FlagTrillion = true;
			GenerateDigitsInfo(digits, value);
			digits.Add(new TrillionDigitInfo(GenerateTrillionProvider(), 0));
			FlagTrillion = false;
		}
		protected internal override void GenerateBillionDigits(DigitInfoCollection digits, long value) {
			FlagBillion = true;
			GenerateDigitsInfo(digits, value);
			digits.Add(new BillionDigitInfo(GenerateBillionProvider(), 0));
			FlagBillion = false;
		}
		protected internal override void GenerateMillionDigits(DigitInfoCollection digits, long value) {
			FlagMillion = true;
			GenerateDigitsInfo(digits, value);
			digits.Add(new MillionDigitInfo(GenerateMillionProvider(), 0));
			FlagMillion = false;
		}
		protected internal override void GenerateThousandSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0 && value < 30 || value > 99) {
				digits.Add(new SeparatorDigitInfo(provider, 2));
				return;
			}
			if (value % 10 == 0 && value < 100)
				digits.Add(new SeparatorDigitInfo(provider, 0));
		}
		protected internal override void GenerateHundredDigits(DigitInfoCollection digits, long value) {
			if (digits.Count != 0 && IsDigitInfoGreaterHundred(digits.Last))
				digits.Add(new SeparatorDigitInfo(new OrdinalSpanishOptionalNumericsProvider(), 0));
			if (FlagQuadrillion || FlagTrillion || FlagBillion || FlagMillion || FlagThousand) {
				if (value == 1) {
					int index = OneHundredValue ? 10 : 1;
					digits.Add(new HundredsDigitInfo(new OrdinalSpanishOptionalNumericsProvider(), index));
				}
				else
					digits.Add(new HundredsDigitInfo(new OrdinalSpanishOptionalNumericsProvider(), value));
				return;
			}
			digits.Add(new HundredsDigitInfo(new OrdinalSpanishNumericsProvider(), value));
		}
		protected internal override void GenerateTenthsDigits(DigitInfoCollection digits, long value) {
			if (digits.Count != 0) {
				if (FlagThousand && IsDigitInfoGreaterThousand(digits.Last))
					digits.Add(new SeparatorDigitInfo(new CardinalSpanishNumericsProvider(), 0));
				DigitType type = digits.Last.Type;
				if (FlagMillion && (type == DigitType.Billion || type == DigitType.Trillion ||
					type == DigitType.Quadrillion || type == DigitType.Quintillion))
					digits.Add(new SeparatorDigitInfo(new CardinalSpanishNumericsProvider(), 0));
				if (FlagBillion && (type == DigitType.Trillion || type == DigitType.Quadrillion || type == DigitType.Quintillion))
					digits.Add(new SeparatorDigitInfo(new CardinalSpanishNumericsProvider(), 0));
				if (FlagTrillion && (type == DigitType.Quadrillion || type == DigitType.Quintillion))
					digits.Add(new SeparatorDigitInfo(new CardinalSpanishNumericsProvider(), 0));
				if (FlagQuadrillion && type == DigitType.Quintillion)
					digits.Add(new SeparatorDigitInfo(new CardinalSpanishNumericsProvider(), 0));
			}
			if (!FlagThousand && !FlagMillion && !FlagBillion && !FlagTrillion && !FlagQuadrillion) {
				base.GenerateTenthsDigits(digits, value);
				return;
			}
			if (value < 30) {
				long index = value == 20 ? 2 : value % 10 + 1;
				digits.Add(new TenthsDigitInfo(GetTenthsProvider(value), index));
			}
			else {
				if (IsValueGreaterThousand) {
					digits.Add(new TenthsDigitInfo(GetTenthsProvider(value), value / 10));
					if (value % 10 != 0)
						GenerateSinglesDigits(digits, value % 10);
					return;
				}
				digits.Add(new TenthsDigitInfo(GetTenthsProvider(value), value / 10));
				GenerateSinglesDigits(digits, value % 10);
			}
		}
		protected internal override void GenerateTeensDigits(DigitInfoCollection digits, long value) {
			if (IsValueGreaterHundred) {
				if (digits.Count != 0)
					GenerateTeensSeparator(digits, GenerateTeensProvider(), value);
				digits.Add(new TeensDigitInfo(new OrdinalSpanishOptionalNumericsProvider(), value));
			}
			else
				base.GenerateTeensDigits(digits, value);
		}
		protected internal override void GenerateTeensSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0 && IsDigitInfoGreaterThousand(digits.Last)) {
				digits.Add(new SeparatorDigitInfo(GenerateTeensProvider(), 0));
				return;
			}
			if (digits.Count != 0 && IsValueGreaterHundred)
				digits.Add(new SeparatorDigitInfo(GenerateTeensProvider(), 2));
			else
				base.GenerateTeensSeparator(digits, provider, value);
		}
		protected internal override void GenerateSinglesSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (FlagThousand && digits.Count != 0 && digits.Last.Type == DigitType.Tenth)
				digits.Add(new SeparatorDigitInfo(new OrdinalSpanishOptionalNumericsProvider(), 1));
			else
				base.GenerateSinglesSeparator(digits, provider, value);
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if (digits.Count == 0 && value == 1 && IsValueGreaterThousand)
				return;
			if (digits.Count == 0 && value > 1 && IsValueGreaterThousand) {
				digits.Add(new SingleNumeralDigitInfo(new CardinalSpanishNumericsProvider(), value));
				return;
			}
			if (digits.Count != 0 && digits.Last.Type == DigitType.Tenth && IsValueGreaterThousand) {
				digits.Add(new SeparatorDigitInfo(new OrdinalSpanishOptionalNumericsProvider(), 1));
				digits.Add(new SingleNumeralDigitInfo(new CardinalSpanishNumericsProvider(), value));
				return;
			}
			if (digits.Count != 0 && digits.Last.Type == DigitType.Billion && IsValueGreaterThousand) {
				digits.Add(new SeparatorDigitInfo(new OrdinalSpanishOptionalNumericsProvider(), 0));
				digits.Add(new SingleNumeralDigitInfo(new CardinalSpanishNumericsProvider(), value));
				return;
			}
			if (digits.Count != 0 && digits.Last.Type == DigitType.Hundred && IsValueGreaterHundred) {
				long index = value == 1 ? value * 10 : value;
				digits.Add(new SingleNumeralDigitInfo(GenerateSinglesProvider(), index));
			}
			else
				base.GenerateSinglesDigits(digits, value);
		}
		protected internal INumericsProvider GetTenthsProvider(long value) {
			if (value > 20 && value < 30)
				return new CardinalSpanishOptionalNumericsProvider();
			return new CardinalSpanishNumericsProvider();
		}
	}
	#endregion
	#region OrdinalSpanishNumberConverter
	public class OrdinalSpanishNumberConverter : OrdinalBasedNumberConverter {
		protected internal override NumberingFormat Type { get { return NumberingFormat.Ordinal; } }
		public override string ConvertNumberCore(long value) {
			return String.Format("{0}°", value);
		}
	}
	#endregion
}
