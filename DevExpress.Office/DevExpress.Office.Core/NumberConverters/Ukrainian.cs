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
	#region CardinalUkrainianNumericsProvider
	public class CardinalUkrainianNumericsProvider : INumericsProvider {
		static string[] separator { get { return new string[] { " ", " " }; } }
		static string[] singleNumeral = { "одна", "дві", "три", "чотири", "п'ять", "шість", "сім", "вісім", "дев'ять" };
		static string[] singles = { "один", "два", "три", "чотири", "п'ять", "шість", "сім", "вісім", "дев'ять", "нуль" };
		static string[] teens = { "десять", "одинадцять", "дванадцять", "тринадцять", "чотирнадцять", "п'ятнадцять", "шістнадцять", "сімнадцять", "вісімнадцять", "дев'ятнадцять" };
		static string[] tenths = { "двадцять", "тридцять", "сорок", "п'ятдесят", "шістдесят", "сімдесят", "вісімдесят", "дев'яносто" };
		static string[] hundreds = { "сто", "двісті", "триста", "чотириста", "п'ятсот", "шістсот", "сімсот", "вісімсот", "дев'ятсот" };
		static string[] thousands = { "тисяча", "тисячі", "тисяч" };
		static string[] million = { "мільйон", "мільйона", "мільйонов" };
		static string[] billion = { "мільярд", "мільярда", "мільярдов" };
		static string[] trillion = { "трильйон", "трильйона", "трильйонов" };
		static string[] quadrillion = { "квадрильйон", "квадрильйона", "квадрильйонов" };
		static string[] quintillion = { "квінтильйон", "квінтильйона", "квінтильйонов" };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return singleNumeral; } }
		public string[] Singles { get { return singles; } }
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
	#region OrdinalUkrainianNumericsProvider
	public class OrdinalUkrainianNumericsProvider : INumericsProvider {
		static string[] generalSingles = { "перший", "другий", "третій", "четвертий", "п'ятий", "шостий", "сьомий", "восьмий", "дев'ятий", "нульовий" };
		static string[] separator = { " ", " " };
		static string[] teens = { "десятий", "одинадцятий", "дванадцятий", "тринадцятий", "чотирнадцятий", "п'ятнадцятий", "шістнадцятий", "сімнадцятий", "вісімнадцятий", "дев'ятнадцятий" };
		static string[] tenths = { "двадцятий", "тридцятий", "сороковий", "п'ятдесятий", "шістдесятий", "сімдесятий", "вісімдесятий", "дев'яностий" };
		static string[] hundreds = { "сотий", "двохсотий", "трьохсотий", "чотирьохсотий", "п'ятисотий", "шестисотий", "семисотий", "восьмисотий", "дев'ятисотий" };
		static string[] thousands = { "тисячний", "тисячний", "тисячний", "тисячний", "тисячний", "тисячний", "тисячний", "тисячний", "тисячний", "тисячний" };
		static string[] million = { "мільйонний" };
		static string[] billion = { "мільярдний" };
		static string[] trillion = { "трильйонний" };
		static string[] quadrillion = { "квадрильйонний" };
		static string[] quintillion = { "квінтильйонний" };
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
	#region ThousandUkrainianNumericsProvider
	public class ThousandUkrainianNumericsProvider : INumericsProvider {
		static string[] generalSingles = { "одно", "двох", "трьох", "чотирьох", "п'яти", "шести", "семи", "восьми", "дев'яти" };
		static string[] separator = { "", "" };
		static string[] teens = { "десяти", "одинадцяти", "двенадцяти", "тринадцяти", "чотирнадцяти", "п'ятнадцяти", "шістнадцяти", "сімнадцяти", "вісімнадцяти", "дев'ятнадцяти" };
		static string[] tenths = { "двадцяти", "тридцяти", "сорока", "п'ятдесяти", "шістдесяти", "сімдесяти", "вісімдесяти", "дев'яносто" };
		static string[] hundreds = { "сто", "двохсот", "трьохсот", "чотирьохсот", "п'ятисот", "шестисот", "семисот", "восьмисот", "дев'ятисот" };
		static string[] thousands = { "тисячний", "тисячний", "тисячний", "тисячний", "тисячний", "тисячний", "тисячний", "тисячний", "тисячний", "тисячний" };
		static string[] million = { "мільйонний" };
		static string[] billion = { "мільярдний" };
		static string[] trillion = { "трильйонний" };
		static string[] quadrillion = { "квадрильйонний" };
		static string[] quintillion = { "квінтильйонний" };
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
	#region DescriptiveCardinalUkrainianNumberConverter
	public class DescriptiveCardinalUkrainianNumberConverter : SlavicCardinalBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if (IsValueGreaterHundred && value == 1 && digits.Count == 0)
				return;
			base.GenerateSinglesDigits(digits, value);
		}
	}
	#endregion
	#region DescriptiveOrdinalUkrainianNumberConverter
	public class DescriptiveOrdinalUkrainianNumberConverter : SlavicOrdinalBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.OrdinalText; } }
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GetProvider() {
			return new ThousandUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new CardinalUkrainianNumericsProvider();
		}
		protected internal override INumericsProvider GetOrdinalSlavicNumericsProvider() {
			return new OrdinalUkrainianNumericsProvider();
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if (FlagThousand && value == 1 && digits.Count == 0)
				return;
			base.GenerateSinglesDigits(digits, value);
		}
	}
	#endregion
	#region OrdinalUkrainianNumberConverter
	public class OrdinalUkrainianNumberConverter : OrdinalSlavicNumberConverter {
	}
	#endregion
}
