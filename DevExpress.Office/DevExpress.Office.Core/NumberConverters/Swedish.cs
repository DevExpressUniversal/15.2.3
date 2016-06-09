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
	#region CardinalSwedishNumericsProvider
	public class CardinalSwedishNumericsProvider : INumericsProvider {
		static string[] separator = { "", "", " " };
		static string[] singlesNumeral = { "et", "två", "tre", "fyra", "fem", "sex", "sju", "åtta", "nio", "noll" };
		static string[] singles = { "ett", "två", "tre", "fyra", "fem", "sex", "sju", "åtta", "nio", "noll", "en" };
		static string[] teens = { "tio", "elva", "tolv", "tretton", "fjorton", "femton", "sexton", "sjutton", "arton", "nitton" };
		static string[] tenths = { "tjugo", "trettio", "fyrtio", "femtio", "sextio", "sjuttio", "åttio", "nittio" };
		static string[] hundreds = { "etthundra", "tvåhundra", "trehundra", "fyrahundra", "femhundra", "sexhundra", "sjuhundra", "åttahundra", "niohundra" };
		static string[] thousands = { "tusen", "tusen" };
		static string[] million = { "miljon", "miljoner" };
		static string[] billion = { "miljard", "miljarder" };
		static string[] trillion = { "biljon", "biljoner" };
		static string[] quadrillion = { "biljard", "biljarder" };
		static string[] quintillion = { "triljon", "triljoner" };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return singlesNumeral; } }
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
	#region OrdinalSwedishNumericsProvider
	public class OrdinalSwedishNumericsProvider : INumericsProvider {
		static string[] separator = { "", "" };
		static string[] singlesNumeral = { "första", "andra", "tredje", "fjärde", "femte", "sjätte", "sjunde", "åttonde", "nionde" };
		static string[] singles = { "första", "andra", "tredje", "fjärde", "femte", "sjätte", "sjunde", "åttonde", "nionde", "nollte" };
		static string[] teens = { "tionde", "elfte", "tolfte", "trettonde", "fjortonde", "femtonde", "sextonde", "sjuttonde", "artonde", "nittonde" };
		static string[] tenths = { "tjugonde", "trettionde", "fyrtionde", "femtionde", "sextionde", "sjuttionde", "åttionde", "nittionde" };
		static string[] hundreds = { "etthundrade", "tvåhundrade", "trehundrade", "fyrahundrade", "femhundrade", "sexhundrade", "sjuhundrade", "åttahundrade", "niohundrade" };
		static string[] thousands = { "tusende", "tusende" };
		static string[] million = { "miljonte", "miljonte" };
		static string[] billion = { "miljardte", "miljardte" };
		static string[] trillion = { "biljonte", "biljonte" };
		static string[] quadrillion = { "biljardte", "biljardte" };
		static string[] quintillion = { "triljonte", "triljonte" };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return singlesNumeral; } }
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
	#region DescriptiveSwedishNumberConverterBase (abstract class)
	public abstract class DescriptiveSwedishNumberConverterBase : SomeLanguagesBased {
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new CardinalSwedishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new CardinalSwedishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new CardinalSwedishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new CardinalSwedishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new CardinalSwedishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new CardinalSwedishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new CardinalSwedishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new CardinalSwedishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalSwedishNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new CardinalSwedishNumericsProvider();
		}
		protected internal override void GenerateQuintillionDigits(DigitInfoCollection digits, long value) {
			FlagQuintillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerQuintillion)
				digits.Add(new SeparatorDigitInfo(new CardinalSwedishNumericsProvider(), 2));
			FlagQuintillion = false;
		}
		protected internal override void GenerateQuadrillionDigits(DigitInfoCollection digits, long value) {
			FlagQuadrillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerQuadrillion)
				digits.Add(new SeparatorDigitInfo(new CardinalSwedishNumericsProvider(), 2));
			FlagQuadrillion = false;
		}
		protected internal override void GenerateTrillionDigits(DigitInfoCollection digits, long value) {
			FlagTrillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerTrillion)
				digits.Add(new SeparatorDigitInfo(new CardinalSwedishNumericsProvider(), 2));
			FlagTrillion = false;
		}
		protected internal override void GenerateBillionDigits(DigitInfoCollection digits, long value) {
			FlagBillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerBillion)
				digits.Add(new SeparatorDigitInfo(new CardinalSwedishNumericsProvider(), 2));
			FlagBillion = false;
		}
		protected internal override void GenerateMillionDigits(DigitInfoCollection digits, long value) {
			FlagMillion = true;
			GenerateLastDigits(digits, value);
			if (!FlagIntegerMillion)
				digits.Add(new SeparatorDigitInfo(new CardinalSwedishNumericsProvider(), 2));
			FlagMillion = false;
		}
		protected DigitInfo ChooseCardinalProvider(int number) {
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
				digits.Add(new SeparatorDigitInfo(new CardinalSwedishNumericsProvider(), 0));
			else
				digits.Add(new SeparatorDigitInfo(new CardinalSwedishNumericsProvider(), 2));
			if (value % 1000000 > 1)
				digits.Add(ChooseCardinalProvider(1));
			else
				digits.Add(ChooseCardinalProvider(0));
		}
	}
	#endregion
	#region DescriptiveCardinalSwedishNumberConverter
	public class DescriptiveCardinalSwedishNumberConverter : DescriptiveSwedishNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if ((FlagMillion || FlagBillion || FlagTrillion || FlagQuadrillion || FlagQuintillion) && digits.Count == 0) {
				long index = value == 1 ? 11 : value;
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), index));
				return;
			}
			base.GenerateSinglesDigits(digits, value);
		}
	}
	#endregion
	#region DescriptiveOrdinalSwedishNumberConverter
	public class DescriptiveOrdinalSwedishNumberConverter : DescriptiveSwedishNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.OrdinalText; } }
		protected internal override void GenerateDigits(DigitInfoCollection digits, long value) {
			base.GenerateDigits(digits, value);
			digits.Last.Provider = new OrdinalSwedishNumericsProvider();
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if ((FlagMillion || FlagBillion || FlagTrillion || FlagQuadrillion || FlagQuintillion) && digits.Count == 0 && value == 1)
				return;
			base.GenerateSinglesDigits(digits, value);
		}
	}
	#endregion
	#region OrdinalSwedishNumberConverter
	public class OrdinalSwedishNumberConverter : OrdinalBasedNumberConverter {
		protected internal override NumberingFormat Type { get { return NumberingFormat.Ordinal; } }
		public override string ConvertNumberCore(long value) {
			if ((value % 10 == 1) || (value % 10 == 2))
				return String.Format("{0}:a", value);
			else
				return String.Format("{0}:e", value);
		}
	}
	#endregion
}
