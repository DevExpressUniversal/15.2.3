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
using System.Globalization;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Model {
	#region DateTimeFieldFormatter
	public class DateTimeFieldFormatter : SpecificFieldFormatter<DateTime> {
		#region Fields
		readonly string AMPMKeyword = "am/pm";
		readonly string defaultFormat = "M/d/yyyy";
		DateTime dateTime;
		string formatString;
		bool useCurrentCultureDateTimeFormat;
		#endregion
		public bool UseCurrentCultureDateTimeFormat { get { return useCurrentCultureDateTimeFormat; } set { useCurrentCultureDateTimeFormat = value; } }
		public virtual DateTimeFormatInfo FormatInfo { get { return Culture.DateTimeFormat; } }
		protected override string Format(DateTime dateTime, string formatString) {
			this.dateTime = dateTime;
			this.formatString = formatString;
			return FormatCore();
		}
		protected internal virtual string FormatCore() {
			int index = 0;
			int formatLength = this.formatString.Length;
			StringBuilder resultString = new StringBuilder();
			while (index < formatLength)
				index += ProcessNextChar(index, resultString);
			return resultString.ToString();
		}
		protected override string FormatByDefault(DateTime value) {
			string formatString = this.useCurrentCultureDateTimeFormat ? Culture.DateTimeFormat.ShortDatePattern : this.defaultFormat;
			return Format(value, formatString);
		}
		protected internal virtual int ProcessNextChar(int index, StringBuilder resultString) {
			char ch = this.formatString[index];
			DateTimeFormattingItem formattingItem;
			if (TryCreateItem(ch, out formattingItem))
				return ProcessAsFormattingItem(index, formattingItem, resultString);
			if (IsKeyword(AMPMKeyword, index))
				return ProcessAsAMPMKeyword(resultString);
			if (ch == '\'')
				return ProcessAsEmbedText(index, resultString);
			return ProcessAsSingleCharacter(index, resultString);
		}
		protected internal bool TryCreateItem(char formattingChar, out DateTimeFormattingItem result) {
			switch (formattingChar) {
				case 'h':
					result = new Hour12FormattingItem(Culture);
					break;
				case 'H':
					result = new Hour24FormattingItem(Culture);
					break;
				case 'm':
					result = new MinuteFormattingItem(Culture);
					break;
				case 'S':
				case 's':
					result = new SecondFormattingItem(Culture);
					break;
				case 'Y':
				case 'y':
					result = new YearFormattingItem(Culture);
					break;
				case 'M':
					result = new MonthFormattingItem(Culture);
					break;
				case 'D':
				case 'd':
					result = new DayFormattingItem(Culture);
					break;
				default:
					result = null;
					return false;
			}
			return true;
		}
		protected internal virtual int ProcessAsFormattingItem(int index, DateTimeFormattingItem formattingItem, StringBuilder resultString) {
			int sequenceLength = GetCharacterSequenceLength(this.formatString[index], index, CharsAreEqual);
			int patternLength = formattingItem.GetAvailablePatternLength(sequenceLength);
			string result = formattingItem.Format(this.dateTime, patternLength);
			resultString.Append(result);
			return Math.Min(sequenceLength, patternLength);
		}
		protected internal virtual int ProcessAsAMPMKeyword(StringBuilder resultString) {
			string result = (this.dateTime.Hour - 12) >= 0 ? "PM" : "AM";
			resultString.Append(result);
			return AMPMKeyword.Length;
		}
		protected internal virtual int ProcessAsEmbedText(int index, StringBuilder resultString) {
			int startTextIndex = index + 1;
			if (startTextIndex >= (this.formatString.Length - 1))
				return 1;
			int textLength = GetCharacterSequenceLength(this.formatString[index], startTextIndex, CharsAreNotEqual);
			if ((textLength + startTextIndex) == this.formatString.Length)
				ThrowUnmatchedQuotesError();
			resultString.Append(this.formatString, startTextIndex, textLength);
			return textLength + 2;
		}
		protected internal virtual int ProcessAsSingleCharacter(int index, StringBuilder resultString) {
			resultString.Append(this.formatString[index]);
			return 1;
		}
		protected internal virtual bool IsKeyword(string keyword, int index) {
			if (keyword.Length > (this.formatString.Length - index))
				return false;
			string substring = this.formatString.Substring(index, keyword.Length);
			return StringExtensions.CompareInvariantCultureIgnoreCase(keyword, substring) == 0;
		}
		protected delegate bool Predicate(char ch1, char ch2);
		int GetCharacterSequenceLength(char ch, int index, Predicate predicate) {
			int length = this.formatString.Length;
			int nextCharIndex = index + 1;
			while (nextCharIndex < length && predicate(ch, this.formatString[nextCharIndex]))
				nextCharIndex++;
			return nextCharIndex - index;
		}
		bool CharsAreEqual(char ch1, char ch2) {
			return ch1 == ch2;
		}
		bool CharsAreNotEqual(char ch1, char ch2) {
			return ch1 != ch2;
		}
	}
	#endregion
	#region DateTimeFormattingItem (abstract class)
	public abstract class DateTimeFormattingItem {
		readonly CultureInfo culture;
		protected DateTimeFormattingItem(CultureInfo culture) {
			Guard.ArgumentNotNull(culture, "formatInfo");
			this.culture = culture;
		}
		public abstract int[] PatternsLength { get; }
		protected CultureInfo Culture { get { return culture; } }
		protected DateTimeFormatInfo FormatInfo { get { return Culture.DateTimeFormat; } }
		protected System.Globalization.Calendar Calendar { get { return FormatInfo.Calendar; } }
		public abstract string Format(DateTime dateTime, int patternLength);
		public int GetAvailablePatternLength(int patternLength) {
			int count = PatternsLength.Length;
			for (int i = 0; i < count; i++) {
				if (PatternsLength[i] >= patternLength)
					return PatternsLength[i];
			}
			return PatternsLength[count - 1];
		}
	}
	#endregion
	#region NumericFormattingItem (abstract class)
	public abstract class NumericFormattingItem : DateTimeFormattingItem {
		readonly int[] patternsLength = { 1, 2 };
		protected NumericFormattingItem(CultureInfo culture)
			: base(culture) {
		}
		public override int[] PatternsLength { get { return patternsLength; } }
		protected virtual string Format(int value, int patternLength) {
			string result = value.ToString(Culture);
			if (patternLength == 2 && result.Length == 1)
				return "0" + result;
			return result;
		}
	}
	#endregion
	#region CombinedFormattingItem (abstract class)
	public abstract class CombinedFormattingItem : NumericFormattingItem {
		readonly int[] patternsLength = { 1, 2, 3, 4 };
		protected CombinedFormattingItem(CultureInfo culture)
			: base(culture) {
		}
		public override int[] PatternsLength { get { return patternsLength; } }
		public override string Format(DateTime dateTime, int patternLength) {
			if (patternLength <= 2)
				return Format(GetNumericValue(dateTime), patternLength);
			if (patternLength == 3)
				return GetAbbreviatedName(dateTime);
			return GetFullName(dateTime);
		}
		protected abstract int GetNumericValue(DateTime dateTime);
		protected abstract string GetAbbreviatedName(DateTime dateTime);
		protected abstract string GetFullName(DateTime dateTime);
	}
	#endregion
	#region Hour24FormattingItem
	public class Hour24FormattingItem : NumericFormattingItem {
		public Hour24FormattingItem(CultureInfo culture)
			: base(culture) {
		}
		public override string Format(DateTime dateTime, int patternLength) {
			return Format(dateTime.Hour, patternLength);
		}
	}
	#endregion
	#region Hour12FormattingItem
	public class Hour12FormattingItem : NumericFormattingItem {
		public Hour12FormattingItem(CultureInfo culture)
			: base(culture) {
		}
		public override string Format(DateTime dateTime, int patternLength) {
			int hour = dateTime.Hour % 12;
			if (hour == 0)
				hour = 12;
			return Format(hour, patternLength);
		}
	}
	#endregion
	#region MinuteFormattingItem
	public class MinuteFormattingItem : NumericFormattingItem {
		public MinuteFormattingItem(CultureInfo culture)
			: base(culture) {
		}
		public override string Format(DateTime dateTime, int patternLength) {
			return Format(dateTime.Minute, patternLength);
		}
	}
	#endregion
	#region SecondFormattingItem
	public class SecondFormattingItem : NumericFormattingItem {
		public SecondFormattingItem(CultureInfo culture)
			: base(culture) {
		}
		public override string Format(DateTime dateTime, int patternLength) {
			return Format(dateTime.Second, patternLength);
		}
	}
	#endregion
	#region DayFormattingItem
	public class DayFormattingItem : CombinedFormattingItem {
		public DayFormattingItem(CultureInfo culture)
			: base(culture) {
		}
		protected override string GetAbbreviatedName(DateTime dateTime) {
			return FormatInfo.GetAbbreviatedDayName(GetDayOfWeek(dateTime));
		}
		protected override string GetFullName(DateTime dateTime) {
			return FormatInfo.GetDayName(GetDayOfWeek(dateTime));
		}
		protected override int GetNumericValue(DateTime dateTime) {
			return dateTime.Day;
		}
		DayOfWeek GetDayOfWeek(DateTime dateTime) {
			return Calendar.GetDayOfWeek(dateTime);
		}
	}
	#endregion
	#region MonthFormattingItem
	public class MonthFormattingItem : CombinedFormattingItem {
		public MonthFormattingItem(CultureInfo culture)
			: base(culture) {
		}
		protected override string GetAbbreviatedName(DateTime dateTime) {
			return FormatInfo.GetAbbreviatedMonthName(GetNumericValue(dateTime));
		}
		protected override string GetFullName(DateTime dateTime) {
			return FormatInfo.GetMonthName(GetNumericValue(dateTime));
		}
		protected override int GetNumericValue(DateTime dateTime) {
			return dateTime.Month;
		}
	}
	#endregion
	#region YearFormattingItem
	public class YearFormattingItem : DateTimeFormattingItem {
		readonly int[] patternsLength = { 2, 4 };
		public YearFormattingItem(CultureInfo culture)
			: base(culture) {
		}
		public override int[] PatternsLength { get { return patternsLength; } }
		public override string Format(DateTime dateTime, int patternLength) {
			int year = dateTime.Year;
			if (patternLength == 2 && year > 99) {
				int shortYear = year % 100;
				string result = shortYear.ToString(Culture);
				if (result.Length == 1)
					return "0" + result;
				return result;
			}
			return year.ToString(Culture);
		}
	}
	#endregion
}
