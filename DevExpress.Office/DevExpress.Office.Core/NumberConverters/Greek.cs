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
	#region CardinalGreekNumericsProvider
	public class CardinalGreekNumericsProvider : INumericsProvider {
		static string[] separator = { " ", " ", "" };
		static string[] generalSingles = { "ένα", "δύο", "τρία", "τέσσερα", "πέντε", "έξι", "επτά", "οκτώ", "εννέα", "μηδέν" };
		static string[] singlesNumeral = { "ένα", "δύο", "τρεις", "τέσσερις", "πέντε", "έξι", "επτά", "οκτώ", "εννέα", "μία" };
		static string[] teens = { "δέκα", "ένδεκα", "δώδεκα", "δεκατρία", "δεκατέσσερα", "δεκαπέντε", "δεκαέξι", "δεκαεπτά", "δεκαοκτώ", "δεκαεννέα" };
		static string[] tenths = { "είκοσι", "τριάντα", "σαράντα", "πενήντα", "εξήντα", "εβδομήντα", "ογδόντα", "ενενήντα" };
		static string[] hundreds = { "εκατόν", "διακόσια", "τριακόσια", "τετρακόσια", "πεντακόσια", "εξακόσια", "επτακόσια", "οκτακόσια", "εννιακόσια", "εκατό" };
		static string[] thousands = { "χιλιάδες", "χίλια" };
		static string[] million = { "εκατομμύρια", "εκατομμύριο" };
		static string[] billion = { "δισεκατομμύρια", "δισεκατομμύριο" };
		static string[] trillion = { "τρισεκατομμύρια", "τρισεκατομμύριο" };
		static string[] quadrillion = { "τετράκις εκατομμύρια", "τετράκις εκατομμύριο" };
		static string[] quintillion = { "πεντάκις εκατομμύρια", "πεντάκις εκατομμύριο" };
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
	#region CardinalGreekOptionalNumericsProvider
	public class CardinalGreekOptionalNumericsProvider : INumericsProvider {
		static string[] separator = { " ", " ", "" };
		static string[] teens = { "δέκα", "ένδεκα", "δώδεκα", "δεκατρείς", "δεκατέσσερις", "δεκαπέντε", "δεκαέξι", "δεκαεπτά", "δεκαοκτώ", "δεκαεννέα" };
		static string[] tenths = { "είκοσι", "τριάντα", "σαράντα", "πενήντα", "εξήντα", "εβδομήντα", "ογδόντα", "ενενήντα" };
		static string[] hundreds = { "εκατόν", "διακόσιες", "τριακόσιες", "τετρακόσιες", "πεντακόσιες", "εξακόσιες", "επτακόσιες", "οκτακόσιες", "εννιακόσιες", "εκατό" };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return null; } }
		public string[] Singles { get { return null; } }
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
	#region OrdinalGreekNumericsProvider
	public class OrdinalGreekNumericsProvider : INumericsProvider {
		static string[] separator = { " ", " ", "" };
		static string[] generalSingles = { "πρώτο", "δεύτερο", "τρίτο", "τέταρτο", "πέμπτο", "έκτο", "έβδομο", "όγδοο", "ένατο", "μηδενικό" };
		static string[] teens = { "δέκατο", "ενδέκατο", "δωδέκατο", "δέκατο τρίτο", "δέκατο τέταρτο", "δέκατο πέμπτο", "δέκατο έκτο", "δέκατο έβδομο", "δέκατο όγδοο", "δέκατο ένατο" };
		static string[] tenths = { "εικοστό", "τριακοστό", "τεσσαρακοστό", "πεντηκοστό", "εξηκοστό", "εβδομηκοστό", "ογδοηκοστό", "ενενηκοστό" };
		static string[] hundreds = { "εκατοστό", "διακοσιοστό", "τριακοσιοστό", "τετρακοσιοστό", "πεντακοσιοστό", "εξακοσιοστό", "επτακοσιοστό", "οκτακοσιοστό", "εννιακοσιοστό" };
		static string[] thousands = { "χιλιοστό" };
		static string[] million = { "εκατομμυριοστό" };
		static string[] billion = { "δισεκατομμυριοστό" };
		static string[] trillion = { "τρισεκατομμυριοστό" };
		static string[] quadrillion = { "τετράκις εκατομμυριοστό" };
		static string[] quintillion = { "πεντάκις εκατομμυριοστό" };
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
	#region OrdinalGreekOptionalNumericsProvider
	public class OrdinalGreekOptionalNumericsProvider : INumericsProvider {
		static string[] singlesNumeral = { "", "δισ", "τρισ", "τετρακισ", "πεντακισ", "εξακισ", "επτακισ", "οκτακισ", "εννιακισ", "" };
		static string[] teens = { "δεκακισ", "ενδεκακισ", "δωδεκακισ", "δεκατριακισ", "δεκατετρακισ", "δεκαπεντακισ", "δεκαεξακισ", "δεκαεπτακισ", "δεκαοκτακισ", "δεκαεννιακισ" };
		public string[] Separator { get { return null; } }
		public string[] SinglesNumeral { get { return singlesNumeral; } }
		public string[] Singles { get { return null; } }
		public string[] Teens { get { return teens; } }
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
	#region DescriptiveGreekNumberConverterBase (abstract class)
	public abstract class DescriptiveGreekNumberConverterBase : DescriptiveNumberConverterBase {
		#region Properties
		protected internal bool FlagOneFullHundred { get; set; }
		#endregion
		protected internal override void GenerateDigitsInfo(DigitInfoCollection digits, long value) {
			if (value / 1000000000000000000 != 0)
				GenerateQuintillionDigits(digits, value / 1000000000000000000);
			value %= 1000000000000000000;
			if (value / 1000000000000000 != 0)
				GenerateQuadrillionDigits(digits, value / 1000000000000000);
			value %= 1000000000000000;
			if (value / 1000000000000 != 0)
				GenerateTrillionDigits(digits, value / 1000000000000);
			value %= 1000000000000;
			if (value / 1000000000 != 0)
				GenerateBillionDigits(digits, value / 1000000000);
			value %= 1000000000;
			if (value / 1000000 != 0)
				GenerateMillionDigits(digits, value / 1000000);
			value %= 1000000;
			if (value / 1000 != 0)
				GenerateThousandDigits(digits, value / 1000);
			value %= 1000;
			FlagOneFullHundred = value % 100 == 0 && value / 100 == 1;
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
		protected internal override void GenerateTeensDigits(DigitInfoCollection digits, long value) {
			if (FlagThousand) {
				GenerateTeensSeparator(digits, GenerateTeensProvider(), value);
				digits.Add(new TeensDigitInfo(GetTeensOptionProvider(digits.Count), value % 10));
			}
			else
				base.GenerateTeensDigits(digits, value);
		}
		protected internal abstract INumericsProvider GetTeensOptionProvider(long digitCount);
	}
	#endregion
	#region DescriptiveCardinalGreekNumberConverter
	public class DescriptiveCardinalGreekNumberConverter : DescriptiveGreekNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new CardinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new CardinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new CardinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			if (FlagThousand)
				return new CardinalGreekOptionalNumericsProvider();
			return new CardinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new CardinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new CardinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new CardinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new CardinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new CardinalGreekNumericsProvider();
		}
		protected internal override void GenerateQuintillionDigits(DigitInfoCollection digits, long value) {
			FlagQuintillion = true;
			if (value == 1)
				GenerateLastDigits(digits, value);
			else
				base.GenerateQuintillionDigits(digits, value);
			FlagQuintillion = false;
		}
		protected internal override void GenerateQuadrillionDigits(DigitInfoCollection digits, long value) {
			FlagQuadrillion = true;
			if (value == 1)
				GenerateLastDigits(digits, value);
			else
				base.GenerateQuadrillionDigits(digits, value);
			FlagQuadrillion = false;
		}
		protected internal override void GenerateTrillionDigits(DigitInfoCollection digits, long value) {
			FlagTrillion = true;
			if (value == 1)
				GenerateLastDigits(digits, value);
			else
				base.GenerateTrillionDigits(digits, value);
			FlagTrillion = false;
		}
		protected internal override void GenerateBillionDigits(DigitInfoCollection digits, long value) {
			FlagBillion = true;
			if (value == 1)
				GenerateLastDigits(digits, value);
			else
				base.GenerateBillionDigits(digits, value);
			FlagBillion = false;
		}
		protected internal override void GenerateMillionDigits(DigitInfoCollection digits, long value) {
			FlagMillion = true;
			if (value == 1)
				GenerateLastDigits(digits, value);
			else
				base.GenerateMillionDigits(digits, value);
			FlagMillion = false;
		}
		protected internal override void GenerateThousandDigits(DigitInfoCollection digits, long value) {
			if (value == 1) {
				if (digits.Count != 0)
					digits.Add(new SeparatorDigitInfo(GenerateHundredProvider(), 0));
				digits.Add(new ThousandsDigitInfo(GenerateThousandProvider(), 1));
			}
			else
				base.GenerateThousandDigits(digits, value);
		}
		protected internal override void GenerateThousandSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (value != 1)
				base.GenerateThousandSeparator(digits, provider, value);
		}
		protected internal override void GenerateHundredDigits(DigitInfoCollection digits, long value) {
			if (FlagOneFullHundred && value == 1) {
				if (digits.Count != 0)
					digits.Add(new SeparatorDigitInfo(GenerateHundredProvider(), 0));
				digits.Add(new HundredsDigitInfo(GenerateHundredProvider(), 10));
			}
			else
				base.GenerateHundredDigits(digits, value);
		}
		protected internal override void GenerateTeensSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0)
				digits.Add(new SeparatorDigitInfo(provider, 0));
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if (digits.Count != 0 && value == 1 && FlagThousand && IsDigitInfoGreaterHundred(digits.Last))
				digits.Add(new SeparatorDigitInfo(GenerateThousandProvider(), 0));
			else {
				value = (value == 1 && FlagThousand) ? 10 : value;
				base.GenerateSinglesDigits(digits, value);
			}
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
			digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value));
			digits.Add(new SeparatorDigitInfo(GenerateQuintillionProvider(), 0));
			digits.Add(ChooseCardinalProvider(1));
		}
		protected internal override INumericsProvider GetTeensOptionProvider(long digitCount) {
			return new CardinalGreekOptionalNumericsProvider();
		}
	}
	#endregion
	#region DescriptiveOrdinalGreekNumberConverter
	public class DescriptiveOrdinalGreekNumberConverter : DescriptiveGreekNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.OrdinalText; } }
		protected internal override INumericsProvider GenerateSinglesProvider() {
			return new OrdinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTeensProvider() {
			return new OrdinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTenthsProvider() {
			return new OrdinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateHundredProvider() {
			return new OrdinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateThousandProvider() {
			return new OrdinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateMillionProvider() {
			return new OrdinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateBillionProvider() {
			return new OrdinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateTrillionProvider() {
			return new OrdinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuadrillionProvider() {
			return new OrdinalGreekNumericsProvider();
		}
		protected internal override INumericsProvider GenerateQuintillionProvider() {
			return new OrdinalGreekNumericsProvider();
		}
		protected internal override void GenerateTrillionSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0 && value != 1)
				base.GenerateTrillionSeparator(digits, provider, value);
		}
		protected internal override void GenerateBillionSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count == 0 && value == 1)
				digits.Add(new SeparatorDigitInfo(provider, 2));
			else
				base.GenerateBillionSeparator(digits, provider, value);
		}
		protected internal override void GenerateMillionSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count == 0 && value == 1)
				digits.Add(new SeparatorDigitInfo(provider, 2));
			else
				base.GenerateMillionSeparator(digits, provider, value);
		}
		protected internal override void GenerateThousandSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0 && value < 20)
				digits.Add(new SeparatorDigitInfo(provider, 2));
			else
				base.GenerateThousandSeparator(digits, provider, value);
		}
		protected internal override void GenerateHundredDigits(DigitInfoCollection digits, long value) {
			if (digits.Count != 0 && FlagThousand) {
				digits.Add(new SeparatorDigitInfo(GenerateHundredProvider(), 0));
				digits.Add(new HundredsDigitInfo(GenerateHundredProvider(), value));
			}
			else
				base.GenerateHundredDigits(digits, value);
		}
		protected internal override void GenerateTenthsDigits(DigitInfoCollection digits, long value) {
			if (FlagThousand && value % 10 == 0) {
				if (digits.Count != 0)
					digits.Add(new SeparatorDigitInfo(GenerateTenthsProvider(), 0));
				digits.Add(new TenthsDigitInfo(GenerateTenthsProvider(), value / 10));
			}
			else
				base.GenerateTenthsDigits(digits, value);
		}
		protected internal override void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if (IsValueGreaterHundred && digits.Count != 0 && IsDigitInfoGreaterThousand(digits.Last)) {
				digits.Add(new SeparatorDigitInfo(GenerateTenthsProvider(), 0));
				digits.Add(new SingleNumeralDigitInfo(new OrdinalGreekOptionalNumericsProvider(), value));
				return;
			}
			if (digits.Count == 0 && IsValueGreaterHundred) {
				if (value == 1)
					return;
				digits.Add(new SingleNumeralDigitInfo(new OrdinalGreekOptionalNumericsProvider(), value));
			}
			else
				base.GenerateSinglesDigits(digits, value);
		}
		protected internal override INumericsProvider GetTeensOptionProvider(long digitCount) {
			if (digitCount == 0)
				return new OrdinalGreekOptionalNumericsProvider();
			return new OrdinalGreekNumericsProvider();
		}
	}
	#endregion
	#region OrdinalGreekNumberConverter
	public class OrdinalGreekNumberConverter : OrdinalBasedNumberConverter {
		protected internal override NumberingFormat Type { get { return NumberingFormat.Ordinal; } }
		public override string ConvertNumberCore(long value) {
			return String.Format("{0}ο", value);
		}
	}
	#endregion
}
