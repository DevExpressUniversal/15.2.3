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
	#region CardinalTurkishNumericsProvider
	public class CardinalTurkishNumericsProvider : INumericsProvider {
		static string[] separator = { "", "", " " };
		static string[] generalSingles = { "bir", "iki", "üç", "dört", "beş", "altı", "yedi", "sekiz", "dokuz", "sıfır" };
		static string[] teens = { "on", "onbir", "oniki", "onüç", "ondört", "onbeş", "onaltı", "onyedi", "onsekiz", "ondokuz" };
		static string[] tenths = { "yirmi", "otuz", "kırk", "elli", "altmış", "yetmiş", "seksen", "doksan" };
		static string[] hundreds = { "yüz", "ikiyüz", "üçyüz", "dörtyüz", "beşyüz", "altıyüz", "yediyüz", "sekizyüz", "dokuzyüz" };
		static string[] thousands = { "bin", "bin" };
		static string[] million = { "milyon", "milyon" };
		static string[] billion = { "milyar", "milyar" };
		static string[] trillion = { "trilyon", "trilyon" };
		static string[] quadrillion = { "katrilyon", "katrilyon" };
		static string[] quintillion = { "kentilyon", "kentilyon" };
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
	#region OrdinalTurkishNumericsProvider
	public class OrdinalTurkishNumericsProvider : INumericsProvider {
		static string[] generalSingles = { "birinci", "ikinci", "üçüncü", "dördüncü", "beşinci", "altıncı", "yedinci", "sekizinci", "dokuzuncu", "sıfırıncı" };
		static string[] separator = { "", "" };
		static string[] teens = { "onuncu", "onbirinci", "onikinci", "onüçüncü", "ondördüncü", "onbeşinci", "onaltıncı", "onyedinci", "onsekizinci", "ondokuzuncu" };
		static string[] tenths = { "yirminci", "otuzuncu", "kırkıncı", "ellinci", "altmışıncı", "yetmişinci", "sekseninci", "doksanıncı" };
		static string[] hundreds = { "yüzüncü", "ikiyüzüncü", "üçyüzüncü", "dörtyüzüncü", "beşyüzüncü", "altıyüzüncü", "yediyüzüncü", "sekizyüzüncü", "dokuzyüzüncü" };
		static string[] thousands = { "bininci", "bininci" };
		static string[] million = { "milyonuncu", "milyonuncu" };
		static string[] billion = { "milyarıncı", "milyarıncı" };
		static string[] trillion = { "trilyonuncu", "trilyonuncu" };
		static string[] quadrillion = { "katrilyonuncu", "katrilyonuncu" };
		static string[] quintillion = { "kentilyonuncu", "kentilyonuncu" };
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
	#region DescriptiveTurkishNumberConverterBase (abstract class)
	public abstract class DescriptiveTurkishNumberConverterBase : SomeLanguagesBased {
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new CardinalTurkishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new CardinalTurkishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new CardinalTurkishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new CardinalTurkishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new CardinalTurkishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new CardinalTurkishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new CardinalTurkishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new CardinalTurkishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalTurkishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new CardinalTurkishNumericsProvider();
		}
		protected internal override void GenerateQuintillionDigits(DigitInfoCollection digits, long value) {
			FlagQuintillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerQuintillion)
				digits.Add(new SeparatorDigitInfo(new CardinalTurkishNumericsProvider(), 2));
			FlagQuintillion = false;
		}
		protected internal override void GenerateQuadrillionDigits(DigitInfoCollection digits, long value) {
			FlagQuadrillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerQuadrillion)
				digits.Add(new SeparatorDigitInfo(new CardinalTurkishNumericsProvider(), 2));
			FlagQuadrillion = false;
		}
		protected internal override void GenerateTrillionDigits(DigitInfoCollection digits, long value) {
			FlagTrillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerTrillion)
				digits.Add(new SeparatorDigitInfo(new CardinalTurkishNumericsProvider(), 2));
			FlagTrillion = false;
		}
		protected internal override void GenerateBillionDigits(DigitInfoCollection digits, long value) {
			FlagBillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerBillion)
				digits.Add(new SeparatorDigitInfo(new CardinalTurkishNumericsProvider(), 2));
			FlagBillion = false;
		}
		protected internal override void GenerateMillionDigits(DigitInfoCollection digits, long value) {
			FlagMillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerMillion)
				digits.Add(new SeparatorDigitInfo(new CardinalTurkishNumericsProvider(), 2));
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
			if (Type == NumberingFormat.OrdinalText && value == 1)
				digits.Add(new SeparatorDigitInfo(new CardinalTurkishNumericsProvider(), 0));
			else
				digits.Add(new SeparatorDigitInfo(new CardinalTurkishNumericsProvider(), 2));
			if (value % 1000000 > 1)
				digits.Add(GetCardinalProvider(1));
			else
				digits.Add(GetCardinalProvider(0));
		}
	}
	#endregion
	#region DescriptiveCardinalTurkishNumberConverter
	public class DescriptiveCardinalTurkishNumberConverter : DescriptiveTurkishNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
	}
	#endregion
	#region DescriptiveOrdinalTurkishNumberConverter
	public class DescriptiveOrdinalTurkishNumberConverter : DescriptiveTurkishNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.OrdinalText; } }
		protected internal override void GenerateDigits(DigitInfoCollection digits, long value) {
			base.GenerateDigits(digits, value);
			digits.Last.Provider = new OrdinalTurkishNumericsProvider();
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if ((FlagMillion || FlagBillion || FlagTrillion || FlagQuadrillion || FlagQuintillion) && digits.Count == 0 && value == 1)
				return;
			base.GenerateSinglesDigits(digits, value);
		}
	}
	#endregion
	#region OrdinalTurkishNumberConverter
	public class OrdinalTurkishNumberConverter : OrdinalBasedNumberConverter {
		protected internal override NumberingFormat Type { get { return NumberingFormat.Ordinal; } }
		public override string ConvertNumberCore(long value) {
			return String.Format("{0}.", value);
		}
	}
	#endregion
}
