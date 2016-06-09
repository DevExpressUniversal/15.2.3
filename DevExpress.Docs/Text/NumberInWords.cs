#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using DevExpress.Office.NumberConverters;
namespace DevExpress.Docs.Text {
	#region NumberCulture
	public enum NumberCulture {
		English,
		English_UnitedKingdom,
		Roman,
		French,
		German,
		Italian,
		Russian,
		Spanish,
		Swedish,
		Turkish,
		Greek,
		Portuguese,
		Ukrainian,
		Hindi,
		Thai,
		LatinLetter,
		RussianLetter,
	}
	#endregion
	#region INumberInWordsProvider
	public interface INumberInWordsProvider {
		string ConvertToText(long value);
		string ConvertToText(long value, CultureInfo culture);
		string ConvertToText(long value, NumberCulture language);
	}
	#endregion
	#region NumberInWords
	public static class NumberInWords {
		static readonly INumberInWordsProvider cardinalConverter = new CardinalSpellNumber();
		static readonly INumberInWordsProvider ordinalConverter = new OrdinalSpellNumber();
#if !SL
	[DevExpressDocsLocalizedDescription("NumberInWordsCardinal")]
#endif
		public static INumberInWordsProvider Cardinal { get { return cardinalConverter; } }
#if !SL
	[DevExpressDocsLocalizedDescription("NumberInWordsOrdinal")]
#endif
		public static INumberInWordsProvider Ordinal { get { return ordinalConverter; } }
	}
	#endregion
	#region SpellNumberBase (abstract class)
	abstract class SpellNumberBase : INumberInWordsProvider {
		static readonly Dictionary<string, NumberCulture> cultureTable = CreateCultureTable();
		static Dictionary<string, NumberCulture> CreateCultureTable() {
			Dictionary<string, NumberCulture> result = new Dictionary<string, NumberCulture>();
			result.Add("English", NumberCulture.English);
			result.Add("English (United Kingdom)", NumberCulture.English_UnitedKingdom);
			result.Add("French", NumberCulture.French);
			result.Add("German", NumberCulture.German);
			result.Add("Italian", NumberCulture.Italian);
			result.Add("Russian", NumberCulture.Russian);
			result.Add("Spanish", NumberCulture.Spanish);
			result.Add("Swedish", NumberCulture.Swedish);
			result.Add("Turkish", NumberCulture.Turkish);
			result.Add("Greek", NumberCulture.Greek);
			result.Add("Portuguese", NumberCulture.Portuguese);
			result.Add("Ukrainian", NumberCulture.Ukrainian);
			result.Add("Hindi", NumberCulture.Hindi);
			result.Add("Thai", NumberCulture.Thai);
			return result;
		}
		public string ConvertToText(long value) {
			return ConvertToText(value, CultureInfo.CurrentCulture);
		}
		public string ConvertToText(long value, CultureInfo culture) {
			NumberCulture NumberCulture;
			if (!cultureTable.TryGetValue(culture.EnglishName, out NumberCulture))
				if (!cultureTable.TryGetValue(culture.Parent.EnglishName, out NumberCulture))
					NumberCulture = NumberCulture.English;
			return ConvertToText(value, NumberCulture);
		}
		public string ConvertToText(long value, NumberCulture language) {
			OrdinalBasedNumberConverter converter = CreateConverter(language);
			return converter.ConvertNumber(value);
		}
		protected abstract OrdinalBasedNumberConverter CreateConverter(NumberCulture numberCulture);
	}
	#endregion
	#region CardinalSpellNumber
	class CardinalSpellNumber : SpellNumberBase {
		protected override OrdinalBasedNumberConverter CreateConverter(NumberCulture numberCulture) {
			switch (numberCulture) {
				default:
				case NumberCulture.English:
					return new DescriptiveCardinalEnglishNumberConverter();
				case NumberCulture.English_UnitedKingdom:
					return new DescriptiveCardinalEnglishUKNumberConverter();
				case NumberCulture.French:
					return new DescriptiveCardinalFrenchNumberConverter();
				case NumberCulture.German:
					return new DescriptiveCardinalGermanNumberConverter();
				case NumberCulture.Italian:
					return new DescriptiveCardinalItalianNumberConverter();
				case NumberCulture.Russian:
					return new DescriptiveCardinalRussianNumberConverter();
				case NumberCulture.Spanish:
					return new DescriptiveCardinalSpanishNumberConverter();
				case NumberCulture.Swedish:
					return new DescriptiveCardinalSwedishNumberConverter();
				case NumberCulture.Turkish:
					return new DescriptiveCardinalTurkishNumberConverter();
				case NumberCulture.Greek:
					return new DescriptiveCardinalGreekNumberConverter();
				case NumberCulture.Portuguese:
					return new DescriptiveCardinalPortugueseNumberConverter();
				case NumberCulture.Ukrainian:
					return new DescriptiveCardinalUkrainianNumberConverter();
				case NumberCulture.Hindi:
					return new DescriptiveCardinalHindiNumberConverter();
				case NumberCulture.Thai:
					return new DescriptiveCardinalThaiNumberConverter();
				case NumberCulture.LatinLetter:
					return new UpperLatinLetterNumberConverter();
				case NumberCulture.RussianLetter:
					return new RussianUpperNumberConverter();
				case NumberCulture.Roman:
					return new UpperRomanNumberConverterClassic();
			}
		}
	}
	#endregion
	#region OrdinalSpellNumber
	class OrdinalSpellNumber : SpellNumberBase {
		protected override OrdinalBasedNumberConverter CreateConverter(NumberCulture numberCulture) {
			switch (numberCulture) {
				default:
				case NumberCulture.English:
					return new DescriptiveOrdinalEnglishNumberConverter();
				case NumberCulture.English_UnitedKingdom:
					return new DescriptiveOrdinalEnglishUKNumberConverter();
				case NumberCulture.French:
					return new DescriptiveOrdinalFrenchNumberConverter();
				case NumberCulture.German:
					return new DescriptiveOrdinalGermanNumberConverter();
				case NumberCulture.Italian:
					return new DescriptiveOrdinalItalianNumberConverter();
				case NumberCulture.Russian:
					return new DescriptiveOrdinalRussianNumberConverter();
				case NumberCulture.Spanish:
					return new DescriptiveOrdinalSpanishNumberConverter();
				case NumberCulture.Swedish:
					return new DescriptiveOrdinalSwedishNumberConverter();
				case NumberCulture.Turkish:
					return new DescriptiveOrdinalTurkishNumberConverter();
				case NumberCulture.Greek:
					return new DescriptiveOrdinalGreekNumberConverter();
				case NumberCulture.Portuguese:
					return new DescriptiveOrdinalPortugueseNumberConverter();
				case NumberCulture.Ukrainian:
					return new DescriptiveOrdinalUkrainianNumberConverter();
				case NumberCulture.Hindi:
					return new DescriptiveOrdinalHindiNumberConverter();
				case NumberCulture.Thai:
					return new DescriptiveOrdinalThaiNumberConverter();
				case NumberCulture.LatinLetter:
					return new UpperLatinLetterNumberConverter();
				case NumberCulture.RussianLetter:
					return new RussianUpperNumberConverter();
				case NumberCulture.Roman:
					return new UpperRomanNumberConverterClassic();
			}
		}
	}
	#endregion
}
