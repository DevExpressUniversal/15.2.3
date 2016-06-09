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
	#region CardinalEnglishNumericsProvider
	public class CardinalEnglishNumericsProvider : INumericsProvider {
		static string[] separator = { " ", "-" };
		static string[] generalSingles = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "zero" };
		static string[] teens = { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
		static string[] tenths = { "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
		static string[] hundreds = { "one hundred", "two hundred", "three hundred", "four hundred", "five hundred", "six hundred", "seven hundred", "eight hundred", "nine hundred" };
		static string[] thousands = { "thousand" };
		static string[] million = { "million" };
		static string[] billion = { "billion" };
		static string[] trillion = { "trillion" };
		static string[] quadrillion = { "quadrillion" };
		static string[] quintillion = { "quintillion" };
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
	#region OrdinalEnglishNumericsProvider
	public class OrdinalEnglishNumericsProvider : INumericsProvider {
		static string[] generalSingles = { "first", "second", "third", "fourth", "fifth", "sixth", "seventh", "eighth", "ninth", "zeroth" };
		static string[] separator = { " ", "-" };
		static string[] teens = { "tenth", "eleventh", "twelfth", "thirteenth", "fourteenth", "fifteenth", "sixteenth", "seventeenth", "eighteenth", "nineteenth" };
		static string[] tenths = { "twentieth", "thirtieth", "fortieth", "fiftieth", "sixtieth", "seventieth", "eightieth", "ninetieth" };
		static string[] hundreds = { "one hundredth", "two hundredth", "three hundredth", "four hundredth", "five hundredth", "six hundredth", "seven hundredth", "eight hundredth", "nine hundredth" };
		static string[] thousands = { "thousandth" };
		static string[] million = { "millionth" };
		static string[] billion = { "billionth" };
		static string[] trillion = { "trillionth" };
		static string[] quadrillion = { "quadrillionth" };
		static string[] quintillion = { "quintillionth" };
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
	#region DescriptiveCardinalEnglishNumberConverter
	public class DescriptiveCardinalEnglishNumberConverter : DescriptiveNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
	}
	#endregion
	#region DescriptiveOrdinalEnglishNumberConverter
	public class DescriptiveOrdinalEnglishNumberConverter : DescriptiveNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.OrdinalText; } }
		protected internal override void GenerateDigits(DigitInfoCollection digits, long value) {
			base.GenerateDigits(digits, value);
			digits.Last.Provider = new OrdinalEnglishNumericsProvider();
		}
	}
	#endregion
	#region OrdinalEnglishNumberConverter
	public class OrdinalEnglishNumberConverter : OrdinalBasedNumberConverter {
		static string[] ending = { "st", "nd", "rd", "th" };
		protected internal override NumberingFormat Type { get { return NumberingFormat.Ordinal; } }
		public override string ConvertNumberCore(long value) {
			long temp = value % 100;
			if (temp < 21)
				switch (temp) {
					case 1: return String.Format("{0}{1}", value, ending[0]);
					case 2: return String.Format("{0}{1}", value, ending[1]);
					case 3: return String.Format("{0}{1}", value, ending[2]);
					default: return String.Format("{0}{1}", value, ending[3]);
				}
			value--;
			temp = value % 10;
			if (temp < 3)
				return String.Format("{0}{1}", value + 1, ending[temp % 3]);
			return String.Format("{0}{1}", value + 1, ending[3]);
		}
	}
	#endregion
}
