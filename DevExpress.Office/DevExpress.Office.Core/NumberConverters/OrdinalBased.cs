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
using System.Collections.Generic;
using DevExpress.Office.Utils;
using DevExpress.Office.Localization;
namespace DevExpress.Office.NumberConverters {
	#region OrdinalBasedNumberConverter (abstract class)
	public abstract class OrdinalBasedNumberConverter {
		protected OrdinalBasedNumberConverter() {
		}
		protected internal abstract NumberingFormat Type { get; }
		protected internal virtual long MinValue { get { return long.MinValue; } }
		protected internal virtual long MaxValue { get { return long.MaxValue; } }
		public string ConvertNumber(long value) {
			if (value < MinValue || value > MaxValue) {
				string message = String.Format(OfficeLocalizer.GetString(OfficeStringId.Msg_InvalidNumberConverterValue), MinValue, MaxValue);
				Exceptions.ThrowArgumentOutOfRangeException("value", message);
			}
			return ConvertNumberCore(value);
		}
		public abstract string ConvertNumberCore(long value);
		#region GetOrdinalNumberConverterByLanguage
		static OrdinalBasedNumberConverter GetOrdinalNumberConverterByLanguage(LanguageId id) {
			if (id == LanguageId.English)
				return new OrdinalEnglishNumberConverter();
			if (id == LanguageId.French)
				return new OrdinalFrenchNumberConverter();
			if (id == LanguageId.German)
				return new OrdinalGermanNumberConverter();
			if (id == LanguageId.Italian)
				return new OrdinalItalianNumberConverter();
			if (id == LanguageId.Russian)
				return new OrdinalRussianNumberConverter();
			if (id == LanguageId.Swedish)
				return new OrdinalSwedishNumberConverter();
			if (id == LanguageId.Turkish)
				return new OrdinalTurkishNumberConverter();
			if (id == LanguageId.Greek)
				return new OrdinalGreekNumberConverter();
			if (id == LanguageId.Spanish)
				return new OrdinalSpanishNumberConverter();
			if (id == LanguageId.Portuguese)
				return new OrdinalPortugueseNumberConverter();
			if (id == LanguageId.Ukrainian)
				return new OrdinalUkrainianNumberConverter();
			return new DecimalNumberConverter();
		}
		#endregion
		#region GetDescriptiveCardinalNumberConverterByLanguage
		static OrdinalBasedNumberConverter GetDescriptiveCardinalNumberConverterByLanguage(LanguageId id) {
			if (id == LanguageId.English)
				return new DescriptiveCardinalEnglishNumberConverter();
			if (id == LanguageId.French)
				return new DescriptiveCardinalFrenchNumberConverter();
			if (id == LanguageId.German)
				return new DescriptiveCardinalGermanNumberConverter();
			if (id == LanguageId.Italian)
				return new DescriptiveCardinalItalianNumberConverter();
			if (id == LanguageId.Russian)
				return new DescriptiveCardinalRussianNumberConverter();
			if (id == LanguageId.Swedish)
				return new DescriptiveCardinalSwedishNumberConverter();
			if (id == LanguageId.Turkish)
				return new DescriptiveCardinalTurkishNumberConverter();
			if (id == LanguageId.Greek)
				return new DescriptiveCardinalGreekNumberConverter();
			if (id == LanguageId.Spanish)
				return new DescriptiveCardinalSpanishNumberConverter();
			if (id == LanguageId.Portuguese)
				return new DescriptiveCardinalPortugueseNumberConverter();
			if (id == LanguageId.Ukrainian)
				return new DescriptiveCardinalUkrainianNumberConverter();
			if (id == LanguageId.Hindi)
				return new DescriptiveCardinalHindiNumberConverter();
			return new DecimalNumberConverter();
		}
		#endregion
		#region GetDescriptiveOrdinalNumberConverterByLanguage
		static OrdinalBasedNumberConverter GetDescriptiveOrdinalNumberConverterByLanguage(LanguageId id) {
			if (id == LanguageId.English)
				return new DescriptiveOrdinalEnglishNumberConverter();
			if (id == LanguageId.French)
				return new DescriptiveOrdinalFrenchNumberConverter();
			if (id == LanguageId.German)
				return new DescriptiveOrdinalGermanNumberConverter();
			if (id == LanguageId.Italian)
				return new DescriptiveOrdinalItalianNumberConverter();
			if (id == LanguageId.Russian)
				return new DescriptiveOrdinalRussianNumberConverter();
			if (id == LanguageId.Swedish)
				return new DescriptiveOrdinalSwedishNumberConverter();
			if (id == LanguageId.Turkish)
				return new DescriptiveOrdinalTurkishNumberConverter();
			if (id == LanguageId.Greek)
				return new DescriptiveOrdinalGreekNumberConverter();
			if (id == LanguageId.Spanish)
				return new DescriptiveOrdinalSpanishNumberConverter();
			if (id == LanguageId.Portuguese)
				return new DescriptiveOrdinalPortugueseNumberConverter();
			if (id == LanguageId.Ukrainian)
				return new DescriptiveOrdinalUkrainianNumberConverter();
			if (id == LanguageId.Hindi)
				return new DescriptiveOrdinalHindiNumberConverter();
			return new DecimalNumberConverter();
		}
		#endregion
		#region CreateConverter
		public static OrdinalBasedNumberConverter CreateConverter(NumberingFormat format, LanguageId id) {
			switch (format) {
				case NumberingFormat.UpperRoman:
					return new UpperRomanNumberConverterClassic();
				case NumberingFormat.LowerRoman:
					return new LowerRomanNumberConverterClassic();
				case NumberingFormat.UpperLetter:
					return new UpperLatinLetterNumberConverter();
				case NumberingFormat.LowerLetter:
					return new LowerLatinLetterNumberConverter();
				case NumberingFormat.NumberInDash:
					return new NumberInDashNumberConverter();
				case NumberingFormat.DecimalZero:
					return new DecimalZeroNumberConverter();
				case NumberingFormat.Bullet:
					return new BulletNumberConverter();
				case NumberingFormat.Ordinal:
					return GetOrdinalNumberConverterByLanguage(id);
				case NumberingFormat.RussianUpper:
					return new RussianUpperNumberConverter();
				case NumberingFormat.RussianLower:
					return new RussianLowerNumberConverter();
				case NumberingFormat.DecimalEnclosedParenthses:
					return new DecimalEnclosedParenthesesNumberConverter();
				case NumberingFormat.Hex:
					return new HexNumberConverter();
				case NumberingFormat.CardinalText:
					return GetDescriptiveCardinalNumberConverterByLanguage(id);
				case NumberingFormat.OrdinalText:
					return GetDescriptiveOrdinalNumberConverterByLanguage(id);
				case NumberingFormat.Decimal:
					return new DecimalNumberConverter();
				case NumberingFormat.Chicago:
					return new ChicagoNumberConverter();
				default:
					return new DecimalNumberConverter();
			}
		}
		#endregion
		#region GetSupportNumberingFormat
		public static List<NumberingFormat> GetSupportNumberingFormat() {
			List<NumberingFormat> supportFormat = new List<NumberingFormat>();
			supportFormat.Add(NumberingFormat.Decimal);
			supportFormat.Add(NumberingFormat.UpperRoman);
			supportFormat.Add(NumberingFormat.LowerRoman);
			supportFormat.Add(NumberingFormat.UpperLetter);
			supportFormat.Add(NumberingFormat.LowerLetter);
			supportFormat.Add(NumberingFormat.Ordinal);
			supportFormat.Add(NumberingFormat.CardinalText);
			supportFormat.Add(NumberingFormat.OrdinalText);
			supportFormat.Add(NumberingFormat.DecimalZero);
			supportFormat.Add(NumberingFormat.RussianUpper);
			supportFormat.Add(NumberingFormat.RussianLower);
			supportFormat.Add(NumberingFormat.DecimalEnclosedParenthses);
			supportFormat.Add(NumberingFormat.NumberInDash);
			supportFormat.Add(NumberingFormat.Chicago);
			return supportFormat;
		}
		#endregion
	}
	#endregion
}
