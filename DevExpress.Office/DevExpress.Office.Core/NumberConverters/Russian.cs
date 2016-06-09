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
	#region CardinalRussianNumericsProvider
	public class CardinalRussianNumericsProvider : INumericsProvider {
		static string[] separator { get { return new string[] { " ", " " }; } }
		static string[] singleNumeral = { "одна", "две", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };
		static string[] singles = { "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять", "ноль" };
		static string[] teens = { "десять", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать" };
		static string[] tenths = { "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто" };
		static string[] hundreds = { "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот" };
		static string[] thousands = { "тысяча", "тысячи", "тысяч" };
		static string[] million = { "миллион", "миллиона", "миллионов" };
		static string[] billion = { "миллиард", "миллиарда", "миллиардов" };
		static string[] trillion = { "триллион", "триллиона", "триллионов" };
		static string[] quadrillion = { "квадриллион", "квадриллиона", "квадриллионов" };
		static string[] quintillion = { "квинтиллион", "квинтиллиона", "квинтиллионов" };
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
	#region OrdinalRussianNumericsProvider
	public class OrdinalRussianNumericsProvider : INumericsProvider {
		static string[] generalSingles = { "первый", "второй", "третий", "четвертый", "пятый", "шестой", "седьмой", "восьмой", "девятый", "нулевой" };
		static string[] separator = { " ", " " };
		static string[] teens = { "десятый", "одиннадцатый", "двенадцатый", "тринадцатый", "четырнадцатый", "пятнадцатый", "шестнадцатый", "семнадцатый", "восемнадцатый", "девятнадцатый" };
		static string[] tenths = { "двадцатый", "тридцатый", "сороковой", "пятидесятый", "шестидесятый", "семидесятый", "восьмидесятый", "девяностый" };
		static string[] hundreds = { "сотый", "двухсотый", "трехсотый", "четырехсотый", "пятисотый", "шестисотый", "семисотый", "восьмисотый", "девятисотый" };
		static string[] thousands = { "тысячный" };
		static string[] million = { "миллионный" };
		static string[] billion = { "миллиардный" };
		static string[] trillion = { "триллионный" };
		static string[] quadrillion = { "квадриллионный" };
		static string[] quintillion = { "квинтиллионный" };
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
	#region ThousandRussianNumericsProvider
	public class ThousandRussianNumericsProvider : INumericsProvider {
		static string[] generalSingles = { "одно", "двух", "трех", "четырех", "пяти", "шести", "семи", "восьми", "девяти" };
		static string[] separator = { "", "" };
		static string[] teens = { "десяти", "одиннадцати", "двенадцати", "тринадцати", "четырнадцати", "пятнадцати", "шестнадцати", "семнадцати", "восемнадцати", "девятнадцати" };
		static string[] tenths = { "двадцати", "тридцати", "сороко", "пятидесяти", "шестидесяти", "семидесяти", "восьмидесяти", "девяносто" };
		static string[] hundreds = { "сто", "двухсот", "трехсот", "четырехсот", "пятисот", "шестисот", "семисот", "восьмисот", "девятисот" };
		static string[] thousands = { "тысячный" };
		static string[] million = { "миллионный" };
		static string[] billion = { "миллиардный" };
		static string[] trillion = { "триллионный" };
		static string[] quadrillion = { "квадриллионный" };
		static string[] quintillion = { "квинтиллионный" };
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
	#region DescriptiveCardinalRussianNumberConverter
	public class DescriptiveCardinalRussianNumberConverter : SlavicCardinalBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new CardinalRussianNumericsProvider();
		}
	}
	#endregion
	#region DescriptiveOrdinalRussianNumberConverter
	public class DescriptiveOrdinalRussianNumberConverter : SlavicOrdinalBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.OrdinalText; } }
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new CardinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GetOrdinalSlavicNumericsProvider() {
			return new OrdinalRussianNumericsProvider();
		}
		protected internal override INumericsProvider GetProvider() {
			return new ThousandRussianNumericsProvider();
		}
	}
	#endregion
	#region OrdinalRussianNumberConverter
	public class OrdinalRussianNumberConverter : OrdinalSlavicNumberConverter {
	}
	#endregion
}
