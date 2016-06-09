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
using System.Globalization;
#region Mask_Tests
#if DEBUGTEST && !SILVERLIGHT
using NUnit.Framework;
#endif
#endregion
using System.Collections.Generic;
namespace DevExpress.Data.Mask {
	[Flags]
	public enum DateTimePart { None = 0, Date = 1, Time = 2, Both = Date | Time }
	public abstract class DateTimeMaskFormatElement {
		public readonly DateTimePart DateTimePart;
		protected readonly DateTimeFormatInfo DateTimeFormatInfo;
		public abstract string Format(DateTime formattedDateTime);
		public bool Editable { get { return this is DateTimeMaskFormatElementEditable; } }
		protected DateTimeMaskFormatElement(DateTimeFormatInfo dateTimeFormatInfo, DateTimePart dateTimePart) {
			this.DateTimeFormatInfo = dateTimeFormatInfo;
			this.DateTimePart = dateTimePart;
		}
	}
	public class DateTimeMaskFormatElementLiteral : DateTimeMaskFormatElement {
		protected string fLiteral;
		public string Literal { get { return fLiteral; } }
		public override string Format(DateTime formattedDateTime) {
			return Literal;
		}
		public DateTimeMaskFormatElementLiteral(string mask, DateTimeFormatInfo dateTimeFormatInfo)
			: base(dateTimeFormatInfo, DateTimePart.None) {
			this.fLiteral = mask;
		}
	}
	public class DateTimeMaskFormatElementNonEditable : DateTimeMaskFormatElement {
		string fMask;
		public string Mask { get { return fMask; } }
		public override string Format(DateTime formattedDateTime) {
			return formattedDateTime.ToString(fMask.Length == 1 ? '%' + fMask : fMask, DateTimeFormatInfo);
		}
		public DateTimeMaskFormatElementNonEditable(string mask, DateTimeFormatInfo dateTimeFormatInfo, DateTimePart dateTimePart)
			: base(dateTimeFormatInfo, dateTimePart) {
			this.fMask = mask;
		}
	}
	public abstract class DateTimeElementEditor {
		public abstract string DisplayText { get; }
		public abstract bool Insert(string inserted);
		public abstract bool Delete();
		public abstract int GetResult();
		public abstract bool SpinUp();
		public abstract bool SpinDown();
		public abstract bool FinalOperatorInsert { get; }
	}
	public abstract class DateTimeMaskFormatElementEditable : DateTimeMaskFormatElementNonEditable {
		public abstract DateTimeElementEditor CreateElementEditor(DateTime editedDateTime);
		public abstract DateTime ApplyElement(int result, DateTime editedDateTime);
		protected DateTimeMaskFormatElementEditable(string mask, DateTimeFormatInfo dateTimeFormatInfo, DateTimePart dateTimePart) : base(mask, dateTimeFormatInfo, dateTimePart) { }
	}
	public class DateTimeElementEditorAmPm : DateTimeElementEditor {
		public const int AMValue = 0;
		public const int PMValue = 1;
		protected readonly string AMDesignator;
		protected readonly string PMDesignator;
		int fResult;
		string fMask;
		public override string DisplayText {
			get {
				string res = fResult == AMValue ? AMDesignator : PMDesignator;
				if(fMask.Length == 1 && res.Length > 1)
					res = res.Substring(0, 1);
				return res;
			}
		}
		public override bool Insert(string inserted) {
			if(inserted.Length == 0)
				return Delete();
			if(AMDesignator.Length > 0
				&& PMDesignator.Length > 0) {
				if(char.ToUpper(AMDesignator[0]) == char.ToUpper(inserted[0])) {
					fResult = AMValue;
					return true;
				} else if(char.ToUpper(PMDesignator[0]) == char.ToUpper(inserted[0])) {
					fResult = PMValue;
					return true;
				}
			} else if(AMDesignator.Length > 0) {
				fResult = AMValue;
				return true;
			} else if(PMDesignator.Length > 0) {
				fResult = PMValue;
				return true;
			}
			return false;
		}
		public override bool FinalOperatorInsert { get { return true; } }
		public override bool Delete() {
			if(AMDesignator.Length != 0
				&& PMDesignator.Length == 0) {
				if(fResult == PMValue)
					return false;
				fResult = PMValue;
			} else {
				if(fResult == AMValue)
					return false;
				fResult = AMValue;
			}
			return true;
		}
		public override bool SpinUp() {
			fResult = AMValue + PMValue - fResult;
			return true;
		}
		public override bool SpinDown() {
			return SpinUp();
		}
		public override int GetResult() {
			return fResult;
		}
		public DateTimeElementEditorAmPm(string mask, int initialValue, string am, string pm) {
			this.fResult = initialValue;
			this.fMask = mask;
			this.AMDesignator = am;
			this.PMDesignator = pm;
			if(this.AMDesignator == this.PMDesignator) {
				this.AMDesignator = "<" + this.AMDesignator;
				this.PMDesignator = ">" + this.PMDesignator;
			}
		}
	}
	public class DateTimeMaskFormatElement_AmPm : DateTimeMaskFormatElementEditable {
		public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime) {
			return new DateTimeElementEditorAmPm(Mask, editedDateTime.Hour / 12, DateTimeFormatInfo.AMDesignator, DateTimeFormatInfo.PMDesignator);
		}
		public override DateTime ApplyElement(int result, DateTime editedDateTime) {
			return editedDateTime.AddHours(12 * (result - editedDateTime.Hour / 12));
		}
		public DateTimeMaskFormatElement_AmPm(string mask, DateTimeFormatInfo dateTimeFormatInfo) : base(mask, dateTimeFormatInfo, DateTimePart.Time) { }
	}
	public class DateTimeNumericRangeElementEditor : DateTimeElementEditor {
		int fMinValue;
		int fMaxValue;
		int fMinDigits;
		int fMaxDigits;
		int fCurrentValue;
		protected int digitsEntered;
		protected bool Touched { get { return digitsEntered > 0; } }
		public int MinValue { get { return fMinValue; } }
		public int MaxValue { get { return fMaxValue; } }
		public virtual int MinDigits { get { return fMinDigits; } }
		public int MaxDigits { get { return fMaxDigits; } }
		public int CurrentValue { get { return fCurrentValue; } }
		protected void SetUntouchedValue(int newValue) {
			this.fCurrentValue = newValue;
			this.digitsEntered = 0;
		}
		public DateTimeNumericRangeElementEditor(int initialValue, int minValue, int maxValue, int minDigits, int maxDigits)
			: this(minValue, maxValue, minDigits, maxDigits) {
			SetUntouchedValue(initialValue);
		}
		public DateTimeNumericRangeElementEditor(int minValue, int maxValue, int minDigits, int maxDigits)
			: base() {
			this.fMinValue = minValue;
			this.fMaxValue = maxValue;
			this.fMinDigits = minDigits;
			this.fMaxDigits = maxDigits;
			this.SetUntouchedValue(minValue);
		}
		public override string DisplayText { get { return CurrentValue.ToString("d" + MinDigits.ToString("d2", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture); } }
		public override bool Insert(string inserted) {
			if(inserted.Length == 0)
				return Delete();
			string work = Touched ? fCurrentValue.ToString("d", CultureInfo.InvariantCulture) : string.Empty;
			int digitsCurrentlyEntered = 0;
			foreach(char ch in inserted) {
				if(ch >= '0' && ch <= '9') {
					work += ch;
					++digitsCurrentlyEntered;
				} else {
					return false;
				}
			}
			if(work.Length > MaxDigits)
				work = work.Substring(work.Length - MaxDigits);
			while(int.Parse(work, CultureInfo.InvariantCulture) > MaxValue)
				work = work.Substring(1);
			int nextValue = int.Parse(work, CultureInfo.InvariantCulture);
			fCurrentValue = nextValue;
			digitsEntered += digitsCurrentlyEntered;
			return true;
		}
		public override bool Delete() {
			if(CurrentValue == MinValue && Touched == false)
				return false;
			this.SetUntouchedValue(MinValue);
			return true;
		}
		public override bool SpinUp() {
			int work = CurrentValue + 1;
			if(work > MaxValue)
				work = MinValue;
			if(work != CurrentValue || Touched) {
				this.SetUntouchedValue(work);
				return true;
			} else {
				return false;
			}
		}
		public override bool SpinDown() {
			int work = CurrentValue - 1;
			if(work < MinValue)
				work = MaxValue;
			if(work != CurrentValue || Touched) {
				this.SetUntouchedValue(work);
				return true;
			} else {
				return false;
			}
		}
		public override int GetResult() {
			if(CurrentValue >= MinValue && CurrentValue <= MaxValue)
				return CurrentValue;
			else
				return MinValue;
		}
		public override bool FinalOperatorInsert {
			get {
				if(!Touched)
					return false;
				if(CurrentValue > 0 && CurrentValue * 10 > MaxValue)
					return true;
				if(MaxDigits > 0 && digitsEntered >= MaxDigits)
					return true;
				return false;
			}
		}
	}
	public abstract class DateTimeNumericRangeFormatElementEditable: DateTimeMaskFormatElementEditable {
		protected DateTimeNumericRangeFormatElementEditable(string mask, DateTimeFormatInfo dateTimeFormatInfo, DateTimePart dateTimePart) : base(mask, dateTimeFormatInfo, dateTimePart) { }
		protected static DateTime ToLeapYear(DateTime dateTime) {
			DateTime newDateTime = dateTime;
			int increment = 1;
			while(!DateTime.IsLeapYear(newDateTime.Year)) {
				if(newDateTime.Year == DateTime.MaxValue.Year)
					increment = -1;
				newDateTime = newDateTime.AddYears(increment);
			}
			return newDateTime;
		}
	}
	public class DateTimeMaskFormatElement_Min : DateTimeNumericRangeFormatElementEditable {
		public DateTimeMaskFormatElement_Min(string mask, DateTimeFormatInfo dateTimeFormatInfo) : base(mask, dateTimeFormatInfo, DateTimePart.Time) { }
		public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime) {
			return new DateTimeNumericRangeElementEditor(editedDateTime.Minute, 0, 59, Mask.Length == 1 ? 1 : 2, 2);
		}
		public override DateTime ApplyElement(int result, DateTime editedDateTime) {
			return editedDateTime.AddMinutes(result - editedDateTime.Minute);
		}
	}
	public class DateTimeMaskFormatElement_s : DateTimeNumericRangeFormatElementEditable {
		public DateTimeMaskFormatElement_s(string mask, DateTimeFormatInfo dateTimeFormatInfo) : base(mask, dateTimeFormatInfo, DateTimePart.Time) { }
		public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime) {
			return new DateTimeNumericRangeElementEditor(editedDateTime.Second, 0, 59, Mask.Length == 1 ? 1 : 2, 2);
		}
		public override DateTime ApplyElement(int result, DateTime editedDateTime) {
			return editedDateTime.AddSeconds(result - editedDateTime.Second);
		}
	}
	public class DateTimeMaskFormatElement_Millisecond : DateTimeNumericRangeFormatElementEditable {
		int EditorMaxValue {
			get {
				switch(Mask.Length) {
					case 1:
						return 9;
					case 2:
						return 99;
					case 3:
					default:
						return 999;
				}
			}
		}
		int EditorCoeff {
			get {
				switch(Mask.Length) {
					case 1:
						return 100;
					case 2:
						return 10;
					case 3:
					default:
						return 1;
				}
			}
		}
		public DateTimeMaskFormatElement_Millisecond(string mask, DateTimeFormatInfo dateTimeFormatInfo) : base(mask, dateTimeFormatInfo, DateTimePart.Time) { }
		public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime) {
			return new DateTimeNumericRangeElementEditor(editedDateTime.Millisecond / EditorCoeff, 0, EditorMaxValue, Mask.Length, Mask.Length);
		}
		public override DateTime ApplyElement(int result, DateTime editedDateTime) {
			return editedDateTime.AddMilliseconds(result * EditorCoeff - editedDateTime.Millisecond);
		}
	}
	public class DateTimeMaskFormatElement_H24 : DateTimeNumericRangeFormatElementEditable {
		public DateTimeMaskFormatElement_H24(string mask, DateTimeFormatInfo dateTimeFormatInfo) : base(mask, dateTimeFormatInfo, DateTimePart.Time) { }
		public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime) {
			return new DateTimeNumericRangeElementEditor(editedDateTime.Hour, 0, 23, Mask.Length == 1 ? 1 : 2, 2);
		}
		public override DateTime ApplyElement(int result, DateTime editedDateTime) {
			return editedDateTime.AddHours(result - editedDateTime.Hour);
		}
	}
	public class DateTimeMaskFormatElement_h12 : DateTimeNumericRangeFormatElementEditable {
		public DateTimeMaskFormatElement_h12(string mask, DateTimeFormatInfo dateTimeFormatInfo) : base(mask, dateTimeFormatInfo, DateTimePart.Time) { }
		public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime) {
			int initialValue = editedDateTime.Hour % 12;
			if(initialValue == 0)
				initialValue = 12;
			return new DateTimeNumericRangeElementEditor(initialValue, 1, 12, Mask.Length == 1 ? 1 : 2, 2);
		}
		public override DateTime ApplyElement(int result, DateTime editedDateTime) {
			int patchedResult = result == 12 ? 0 : result;
			return editedDateTime.AddHours(patchedResult - (editedDateTime.Hour % 12));
		}
	}
	public class DateTimeMaskFormatElement_d : DateTimeNumericRangeFormatElementEditable {
		DateTimeMaskFormatElementContext context;
		public DateTimeMaskFormatElement_d(string mask, DateTimeFormatInfo dateTimeFormatInfo, DateTimeMaskFormatElementContext context)
			: base(mask, dateTimeFormatInfo, DateTimePart.Date) {
			this.context = context;
		}
		public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime) {
			int year = context.YearProcessed ? editedDateTime.Year : 2004;
			int month = context.MonthProcessed ? editedDateTime.Month : 1;
			return new DateTimeNumericRangeElementEditor(editedDateTime.Day, 1, DateTime.DaysInMonth(year, month), Mask.Length == 1 ? 1 : 2, 2);
		}
		public override DateTime ApplyElement(int result, DateTime editedDateTime) {
			DateTime newDateTime = editedDateTime.AddDays(result - editedDateTime.Day);
			if(newDateTime.Day != result) {
				if(result == 29 && editedDateTime.Month == 2
					&& context.MonthProcessed) {
					newDateTime = editedDateTime;
					newDateTime = ToLeapYear(newDateTime);
					newDateTime = newDateTime.AddDays(result - newDateTime.Day);
				} else {
					newDateTime = editedDateTime.AddMonths(1 - editedDateTime.Month).AddDays(result - editedDateTime.Day);
				}
			}
			return newDateTime;
		}
	}
	public class DateTimeYearElementEditor : DateTimeNumericRangeElementEditor {
		readonly int maskLength;
		readonly DateTimeFormatInfo dateTimeFormatInfo;
		static int GetYearShift(System.Globalization.Calendar calendar) { return calendar.GetYear(new DateTime(2001, 01, 01)) - 2001; }
		public DateTimeYearElementEditor(int initialYear, int maskLength, DateTimeFormatInfo dateTimeFormatInfo)
			:
			base(
			maskLength <= 4 ? 0 : DateTime.MinValue.Year,
			maskLength < 4 ? 99 : DateTime.MaxValue.Year,
			(maskLength == 2) ? 2 : 1,
			maskLength > 3 ? 4 : 2) {
			this.maskLength = maskLength;
			this.dateTimeFormatInfo = dateTimeFormatInfo;
			int calendarYear = initialYear + GetYearShift(dateTimeFormatInfo.Calendar);
			calendarYear = Math.Min(calendarYear, 9999);
			calendarYear = Math.Max(calendarYear, 1);
			calendarYear = maskLength < 4 ? calendarYear % 100 : calendarYear;
			SetUntouchedValue(calendarYear);
		}
		public override int GetResult() {
			int result = base.GetResult();
			if(result < DateTime.MinValue.Year || (maskLength < 4 && result <= 99) || (maskLength == 4 && result <= 99 && digitsEntered <= 2)) {
				try {
					result = dateTimeFormatInfo.Calendar.ToFourDigitYear(result);
				} catch { }
			}
			result -= GetYearShift(dateTimeFormatInfo.Calendar);
			result = Math.Min(result, dateTimeFormatInfo.Calendar.MaxSupportedDateTime.Year);
			result = Math.Max(result, dateTimeFormatInfo.Calendar.MinSupportedDateTime.Year);
			return result;
		}
		public override int MinDigits {
			get {
				if(maskLength == 4) {
					if(digitsEntered < 1)
						return 1;
					if(digitsEntered > 4)
						return 4;
					return digitsEntered;
				} else {
					return base.MinDigits;
				}
			}
		}
	}
	public class DateTimeMaskFormatElement_Year : DateTimeNumericRangeFormatElementEditable {
		public DateTimeMaskFormatElement_Year(string mask, DateTimeFormatInfo dateTimeFormatInfo)
			: base(mask, dateTimeFormatInfo, DateTimePart.Date) {
		}
		public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime) {
			return new DateTimeYearElementEditor(editedDateTime.Year, Mask.Length, DateTimeFormatInfo);
		}
		public override DateTime ApplyElement(int result, DateTime editedDateTime) {
			DateTime newDateTime = editedDateTime.AddYears(result - editedDateTime.Year);
			return newDateTime;
		}
	}
	public class DateTimeMonthElementEditor : DateTimeNumericRangeElementEditor {
		readonly string[] monthsKeys;
		public DateTimeMonthElementEditor(int initialValue, int minDigits, string[] monthsNames)
			: base(initialValue, 1, 12, minDigits, 2) {
			this.monthsKeys = monthsNames;
			if(this.monthsKeys != null && this.monthsKeys[12] != null && this.monthsKeys[12].Length > 0)
				this.monthsKeys = null;
		}
		public override string DisplayText {
			get {
				if(monthsKeys == null || CurrentValue < 1 || CurrentValue > 12)
					return base.DisplayText;
				else
					return monthsKeys[CurrentValue - 1];
			}
		}
		public override bool Insert(string inserted) {
			if(monthsKeys != null) {
				string lowerInput = inserted.ToLower();
				if(inserted.Length > 0) {
					for(int i = 0; i < 12; ++i) {
						int testedMonth = (CurrentValue + i) % 12 + 1;
						if(monthsKeys[testedMonth - 1].ToLower() == lowerInput) {
							SetUntouchedValue(testedMonth);
							return true;
						}
					}
				}
				if(lowerInput.Length == 1) {
					for(int i = 0; i < 12; ++i) {
						int testedMonth = (CurrentValue + i) % 12 + 1;
						string monthName = monthsKeys[testedMonth - 1];
						if(monthName.ToLower().Substring(0, 1) == lowerInput) {
							SetUntouchedValue(testedMonth);
							return true;
						}
					}
				}
			}
			return base.Insert(inserted);
		}
	}
	public class DateTimeMaskFormatElement_Month : DateTimeNumericRangeFormatElementEditable {
		readonly DateTimeMaskFormatElementContext context;
		DateTimeMaskFormatGlobalContext monthNamesDeterminator;
		string[] _MonthNames;
		protected string[] MonthNames {
			get {
				if(monthNamesDeterminator != null) {
					if(Mask.Length == 3) {
						if(monthNamesDeterminator.Value.DayProcessed)
							this._MonthNames = DateTimeFormatInfo.AbbreviatedMonthGenitiveNames;
						else
							this._MonthNames = DateTimeFormatInfo.AbbreviatedMonthNames;
					} else if(Mask.Length > 3) {
						if(monthNamesDeterminator.Value.DayProcessed)
							this._MonthNames = DateTimeFormatInfo.MonthGenitiveNames;
						else
							this._MonthNames = DateTimeFormatInfo.MonthNames;
					} else
						this._MonthNames = null;
					monthNamesDeterminator = null;
				}
				return _MonthNames;
			}
		}
		public DateTimeMaskFormatElement_Month(string mask, DateTimeFormatInfo dateTimeFormatInfo, DateTimeMaskFormatGlobalContext globalContext)
			: base(mask, dateTimeFormatInfo, DateTimePart.Date) {
			this.monthNamesDeterminator = globalContext;
			this.context = globalContext.Value;
		}
		public override string Format(DateTime formattedDateTime) {
			if(MonthNames != null) {
				int month = formattedDateTime.Month;
				return MonthNames[month - 1];
			}
			return base.Format(formattedDateTime);
		}
		public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime) {
			return new DateTimeMonthElementEditor(editedDateTime.Month, Mask.Length == 2 ? 2 : 1, MonthNames);
		}
		public override DateTime ApplyElement(int result, DateTime editedDateTime) {
			DateTime newDateTime = editedDateTime.AddMonths(result - editedDateTime.Month);
			if(result == 2 && editedDateTime.Day == 29 && editedDateTime.Day != newDateTime.Day
				&& context.DayProcessed && !context.YearProcessed) {
				newDateTime = editedDateTime;
				newDateTime = ToLeapYear(newDateTime);
				newDateTime = newDateTime.AddMonths(result - newDateTime.Month);
			}
			return newDateTime;
		}
	}
	public struct DateTimeMaskFormatElementContext {
		public bool YearProcessed;
		public bool MonthProcessed;
		public bool DayProcessed;
	}
	public class DateTimeMaskFormatGlobalContext {
		public DateTimeMaskFormatElementContext Value = new DateTimeMaskFormatElementContext();
	}
	public class DateTimeMaskFormatInfo : IEnumerable<DateTimeMaskFormatElement> {
		public int Count { get { return innerList.Count; } }
		IEnumerator<DateTimeMaskFormatElement> IEnumerable<DateTimeMaskFormatElement>.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		public DateTimeMaskFormatElement this[int index] {
			get {
				return innerList[index];
			}
		}
		protected readonly IList<DateTimeMaskFormatElement> innerList;
		static int GetGroupLength(string mask) {
			for(int i = 1; i < mask.Length; ++i) {
				if(mask[i] != mask[0])
					return i;
			}
			return mask.Length;
		}
		static IList<DateTimeMaskFormatElement> ParseFormatString(string mask, DateTimeFormatInfo dateTimeFormatInfo) {
			List<DateTimeMaskFormatElement> result = new List<DateTimeMaskFormatElement>();
			string work = mask;
			DateTimeMaskFormatGlobalContext context = new DateTimeMaskFormatGlobalContext();
			while(work.Length > 0) {
				int elementLength = GetGroupLength(work);
				DateTimeMaskFormatElement element;
				switch(work[0]) {
					case 'd':
						if(elementLength > 2)
							element = new DateTimeMaskFormatElementNonEditable(work.Substring(0, elementLength), dateTimeFormatInfo, DateTimePart.Date);
						else {
							element = new DateTimeMaskFormatElement_d(work.Substring(0, elementLength), dateTimeFormatInfo, context.Value);
							context.Value.DayProcessed = true;
						}
						break;
					case 'f':
					case 'F':
						if(elementLength > 7) {
							elementLength = 7;
						}
						if(elementLength > 3) {
							element = new DateTimeMaskFormatElementNonEditable(work.Substring(0, elementLength), dateTimeFormatInfo, DateTimePart.Time);
						} else {
							element = new DateTimeMaskFormatElement_Millisecond(work.Substring(0, elementLength), dateTimeFormatInfo);
						}
						break;
					case 'g':
						element = new DateTimeMaskFormatElementNonEditable(work.Substring(0, elementLength), dateTimeFormatInfo, DateTimePart.Date);
						break;
					case 'h':
						element = new DateTimeMaskFormatElement_h12(work.Substring(0, elementLength), dateTimeFormatInfo);
						break;
					case 'H':
						element = new DateTimeMaskFormatElement_H24(work.Substring(0, elementLength), dateTimeFormatInfo);
						break;
					case 'm':
						element = new DateTimeMaskFormatElement_Min(work.Substring(0, elementLength), dateTimeFormatInfo);
						break;
					case 'M':
						if(elementLength > 4)
							elementLength = 4;
						element = new DateTimeMaskFormatElement_Month(work.Substring(0, elementLength), dateTimeFormatInfo, context);
						context.Value.MonthProcessed = true;
						break;
					case 's':
						element = new DateTimeMaskFormatElement_s(work.Substring(0, elementLength), dateTimeFormatInfo);
						break;
					case 't':
						element = new DateTimeMaskFormatElement_AmPm(work.Substring(0, elementLength), dateTimeFormatInfo);
						break;
					case 'y':
						element = new DateTimeMaskFormatElement_Year(work.Substring(0, elementLength), dateTimeFormatInfo);
						context.Value.YearProcessed = true;
						break;
					case 'z':
						element = new DateTimeMaskFormatElementNonEditable(work.Substring(0, elementLength), dateTimeFormatInfo, DateTimePart.Both);
						break;
					case ':':
						elementLength = 1;
						element = new DateTimeMaskFormatElementNonEditable(work.Substring(0, 1), dateTimeFormatInfo, DateTimePart.Time);
						break;
					case '/':
						elementLength = 1;
						element = new DateTimeMaskFormatElementNonEditable(work.Substring(0, 1), dateTimeFormatInfo, DateTimePart.Date);
						break;
					case '"':
					case '\'':
						string escaped = work.Replace(@"\\", @"--").Replace(@"\" + work[0], @"--");
						int closingQuotePosition = escaped.IndexOf(work[0], 1);
						if(closingQuotePosition <= 0)
							throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskNonclosedQuota);
						string quotedFormat = work.Substring(0, closingQuotePosition + 1);
						string formattedQuotedText = DateTime.MinValue.ToString(quotedFormat, dateTimeFormatInfo);
						element = new DateTimeMaskFormatElementLiteral(formattedQuotedText, dateTimeFormatInfo);
						elementLength = closingQuotePosition + 1;
						break;
					case '\\':
						if(work.Length >= 2) {
							element = new DateTimeMaskFormatElementLiteral(work.Substring(1, 1), dateTimeFormatInfo);
							elementLength = 2;
						} else
							throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskBackslashBeforeEndOfMask);
						break;
					default:
						elementLength = 1;
						element = new DateTimeMaskFormatElementLiteral(work.Substring(0, 1), dateTimeFormatInfo);
						break;
				}
				result.Add(element);
				work = work.Substring(elementLength);
			}
			return result;
		}
		static string ExpandFormat(string format, DateTimeFormatInfo info) {
			if(format == null || format.Length == 0)
				format = "G";
			if(format.Length == 1) {
				switch(format[0]) {
					case 'd':
						return info.ShortDatePattern;
					case 'D':
						return info.LongDatePattern;
					case 't':
						return info.ShortTimePattern;
					case 'T':
						return info.LongTimePattern;
					case 'f':
						return info.LongDatePattern + ' ' + info.ShortTimePattern;
					case 'F':
						return info.FullDateTimePattern;
					case 'g':
						return info.ShortDatePattern + ' ' + info.ShortTimePattern;
					case 'G':
						return info.ShortDatePattern + ' ' + info.LongTimePattern;
					case 'm':
					case 'M':
						return info.MonthDayPattern;
					case 'r':
					case 'R':
						return info.RFC1123Pattern;
					case 's':
						return info.SortableDateTimePattern;
					case 'u':
						return info.UniversalSortableDateTimePattern;
					case 'y':
					case 'Y':
						return info.YearMonthPattern;
				}
			}
			if(format.Length == 2 && format[0] == '%')
				format = format.Substring(1);
			return format;
		}
		public DateTimeMaskFormatInfo(string mask, DateTimeFormatInfo dateTimeFormatInfo) {
			string expandedMask = ExpandFormat(mask, dateTimeFormatInfo);
			innerList = ParseFormatString(expandedMask, dateTimeFormatInfo);
		}
		public string Format(DateTime formatted) {
			return Format(formatted, 0, this.Count - 1);
		}
		public string Format(DateTime formatted, int startFormatIndex, int endFormatIndex) {
			string result = string.Empty;
			for(int i = startFormatIndex; i <= endFormatIndex; ++i) {
				result += this[i].Format(formatted);
			}
			return result;
		}
		public DateTimePart DateTimeParts {
			get {
				DateTimePart rv = DateTimePart.None;
				foreach(DateTimeMaskFormatElement e in this) {
					rv |= e.DateTimePart;
				}
				return rv;
			}
		}
		public static string RemoveTimePartFromTheMask(string patchedMask, IFormatProvider formatProvider) {
			if(formatProvider == null)
				formatProvider = DateTimeFormatInfo.CurrentInfo;
			DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)(formatProvider.GetFormat(typeof(DateTimeFormatInfo)));
			if(dateTimeFormatInfo == null)
				dateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;
			string expandedMask = ExpandFormat(patchedMask, dateTimeFormatInfo);
			IList<DateTimeMaskFormatElement> parsed = ParseFormatString(expandedMask, dateTimeFormatInfo);
			string result = string.Empty;
			foreach(DateTimeMaskFormatElement formatElement in parsed) {
				if(formatElement is DateTimeMaskFormatElementLiteral) {
					string literal = ((DateTimeMaskFormatElementLiteral)formatElement).Literal;
					if(literal == " " || literal == "," || literal == ";" || literal == "." || literal == "-") {
						result += literal;
					} else if(literal.Length > 1 && literal.IndexOf('\'') < 0) {
						result += "'" + literal + "'";
					} else if(literal.Length > 1 && literal.IndexOf('\"') < 0) {
						result += "\"" + literal + "\"";
					} else {
						foreach(char ch in literal) {
							result += "\\" + ch;
						}
					}
				} else {
					DateTimeMaskFormatElementNonEditable formattedMaskFormatElement = (DateTimeMaskFormatElementNonEditable)formatElement;
					switch(formattedMaskFormatElement.Mask[0]) {
						case 'f':
						case 'h':
						case 'H':
						case 'm':
						case 's':
						case 't':
						case 'z':
						case ':':
							break;
						default:
							result += formattedMaskFormatElement.Mask;
							break;
					}
				}
			}
			result = result.Trim();
			if(result == expandedMask)
				return patchedMask;
			return result;
		}
	}
	public class DateTimeMaskManagerCore : MaskManager {
		protected readonly bool IsOperatorMask;
		protected DateTime? fCurrentValue;
		protected DateTime? fUndoValue;
		protected DateTime? fInitialEditValue;
		protected DateTimeMaskFormatInfo fFormatInfo;
		protected int fSelectedFormatInfoIndex;
		protected DateTimeElementEditor fCurrentElementEditor;
		protected string fInitialMask;
		protected DateTimeFormatInfo fInitialDateTimeFormatInfo;
		protected readonly bool AllowNull = true;
		protected DateTime? CurrentValue { get { return fCurrentValue; } }
		protected DateTime? UndoValue { get { return fUndoValue; } }
		protected DateTime NonEmptyCurrentValue {
			get {
				if(CurrentValue.HasValue)
					return CurrentValue.Value;
				else
					return GetClearValue();
			}
		}
		protected internal DateTimeMaskFormatInfo FormatInfo { get { return fFormatInfo; } }
		protected int SelectedFormatInfoIndex { get { return fSelectedFormatInfoIndex; } }
		protected DateTimeMaskFormatElementEditable SelectedElement {
			get {
				if(SelectedFormatInfoIndex < 0)
					return null;
				return (DateTimeMaskFormatElementEditable)FormatInfo[SelectedFormatInfoIndex];
			}
		}
		protected bool IsElementEdited { get { return fCurrentElementEditor != null; } }
		protected DateTimeElementEditor GetCurrentElementEditor() {
			if(SelectedElement != null) {
				bool canModify = RaiseModifyWithoutEditValueChange();
				if(!IsElementEdited && canModify) {
					fCurrentElementEditor = SelectedElement.CreateElementEditor(NonEmptyCurrentValue);
				}
			}
			return fCurrentElementEditor;
		}
		protected void KillCurrentElementEditor() {
			fCurrentElementEditor = null;
		}
		protected bool ApplyCurrentElementEditor() {
			if(!IsElementEdited)
				return false;
			int editorResult = GetCurrentElementEditor().GetResult();
			KillCurrentElementEditor();
			DateTime newValue = SelectedElement.ApplyElement(editorResult, NonEmptyCurrentValue);
			if(CurrentValue != newValue) {
				if(!RaiseEditTextChanging(newValue))
					return false;
				fUndoValue = CurrentValue;
				fCurrentValue = newValue;
				RaiseEditTextChanged();
			}
			return true;
		}
		public override string GetCurrentEditText() {
			if(CurrentValue.HasValue)
				return CurrentValue.Value.ToString("G", CultureInfo.InvariantCulture);
			else
				return string.Empty;
		}
		public override void SetInitialEditText(string initialEditText) {
			KillCurrentElementEditor();
			DateTime? initialEditValue = null;
			if(!string.IsNullOrEmpty(initialEditText)) {
				try {
					initialEditValue = DateTime.Parse(initialEditText, CultureInfo.InvariantCulture);
				} catch { }
			}
			SetInitialEditValue(initialEditValue);
		}
		DateTime cachedValue = new DateTime(0);
		int cachedIndex = -1;
		int cachedDCP = -1;
		int cachedDSA = -1;
		string cachedDT = null;
		void VerifyCache() {
			if(!CurrentValue.HasValue)
				throw new InvalidOperationException();
			if(cachedValue != CurrentValue.Value || SelectedFormatInfoIndex != cachedIndex) {
				cachedIndex = SelectedFormatInfoIndex;
				cachedDCP = -1;
				cachedDSA = -1;
			}
			if(cachedValue != CurrentValue.Value) {
				cachedValue = CurrentValue.Value;
				cachedDT = null;
			}
		}
		public override string DisplayText {
			get {
				if(IsElementEdited) {
					return FormatInfo.Format(NonEmptyCurrentValue, 0, SelectedFormatInfoIndex - 1) + GetCurrentElementEditor().DisplayText + FormatInfo.Format(NonEmptyCurrentValue, SelectedFormatInfoIndex + 1, FormatInfo.Count - 1);
				} else if(!CurrentValue.HasValue) {
					return string.Empty;
				} else {
					VerifyCache();
					if(cachedDT == null) {
						cachedDT = FormatInfo.Format(NonEmptyCurrentValue);
					}
					return cachedDT;
				}
			}
		}
		public override int DisplayCursorPosition {
			get {
				if(SelectedElement == null) {
					return 0;
				}
				if(IsElementEdited) {
					return FormatInfo.Format(NonEmptyCurrentValue, 0, SelectedFormatInfoIndex - 1).Length + GetCurrentElementEditor().DisplayText.Length;
				} else if(!CurrentValue.HasValue) {
					return 0;
				} else {
					VerifyCache();
					if(cachedDCP < 0) {
						cachedDCP = FormatInfo.Format(NonEmptyCurrentValue, 0, SelectedFormatInfoIndex).Length;
					}
					return cachedDCP;
				}
			}
		}
		public override int DisplaySelectionAnchor {
			get {
				if(SelectedElement == null) {
					return 0;
				}
				if(IsElementEdited) {
					return FormatInfo.Format(NonEmptyCurrentValue, 0, SelectedFormatInfoIndex - 1).Length;
				}
				if(!CurrentValue.HasValue) {
					return 0;
				}
				VerifyCache();
				if(cachedDSA < 0) {
					cachedDSA = FormatInfo.Format(NonEmptyCurrentValue, 0, SelectedFormatInfoIndex - 1).Length;
				}
				return cachedDSA;
			}
		}
		protected virtual DateTime CorrectInsertValue(DateTime inserted) {
			switch(FormatInfo.DateTimeParts) {
				case DateTimePart.Time:
					return GetClearValue().Date.AddTicks(inserted.TimeOfDay.Ticks);
				case DateTimePart.Date:
					return new DateTime(GetClearValue().TimeOfDay.Ticks + inserted.Date.Ticks);
				default:
					return inserted;
			}
		}
		public override bool Insert(string insertion) {
			if(!IsElementEdited && insertion.Length > 3 && !System.Text.RegularExpressions.Regex.IsMatch(insertion, @"^(\d+|\p{L}+)$")) {
				try {
					DateTime parsedValue = DateTime.ParseExact(insertion, fInitialMask, fInitialDateTimeFormatInfo);
					DateTime newValue = CorrectInsertValue(parsedValue);
					if(newValue == CurrentValue)
						return false;
					if(!RaiseEditTextChanging(newValue))
						return false;
					fUndoValue = CurrentValue;
					fCurrentValue = newValue;
					RaiseEditTextChanged();
					return true;
				} catch {
				}
			}
			bool wasEdited = IsElementEdited;
			DateTimeElementEditor currentEditor = GetCurrentElementEditor();
			if(currentEditor == null)
				return false;
			else if(currentEditor.Insert(insertion)) {
				if(IsOperatorMask && currentEditor.FinalOperatorInsert) {
					CursorRight(false);
				}
				return true;
			} else if(insertion == " ") {
				if(!CurrentValue.HasValue && !wasEdited) {
					return ApplyCurrentElementEditor();
				} else {
					return CursorRight(false);
				}
			} else if(IsNextSeparatorSkipInput(insertion))
				return CursorRight(false);
			else {
				if(!wasEdited)
					KillCurrentElementEditor();
				return false;
			}
		}
		bool IsNextSeparatorSkipInput(string insertion) {
			if(insertion.Length <= 0)
				return false;
			if(SelectedFormatInfoIndex + 1 >= FormatInfo.Count)
				return false;
			if(FormatInfo[SelectedFormatInfoIndex + 1].Editable)
				return false;
			var nextSeparator = FormatInfo[SelectedFormatInfoIndex + 1].Format(NonEmptyCurrentValue);
			return nextSeparator.StartsWith(insertion) || (insertion == "," && nextSeparator.StartsWith("."));
		}
		public override bool Delete() {
			return BackspaceOrDelete();
		}
		public override bool Backspace() {
			return BackspaceOrDelete();
		}
		bool BackspaceOrDelete() {
			if(!CurrentValue.HasValue && !IsElementEdited && AllowNull)
				return false;
			DateTimeElementEditor currentEditor = GetCurrentElementEditor();
			if(currentEditor == null)
				return false;
			else
				return currentEditor.Delete();
		}
		public override bool CanUndo {
			get {
				return CurrentValue != UndoValue || IsElementEdited;
			}
		}
		public override bool Undo() {
			if(IsElementEdited) {
				KillCurrentElementEditor();
			} else {
				if(CurrentValue == UndoValue)
					return false;
				if(!RaiseEditTextChanging(UndoValue))
					return false;
				DateTime? temp = CurrentValue;
				fCurrentValue = UndoValue;
				fUndoValue = temp;
				RaiseEditTextChanged();
			}
			return true;
		}
		int GetFormatIndexFromPosition(int position) {
			int lastElement = -1;
			for(int i = 0; i < FormatInfo.Count; ++i) {
				if(FormatInfo[i].Editable) {
					if(FormatInfo.Format(NonEmptyCurrentValue, 0, i).Length >= position) {
						return i;
					}
					lastElement = i;
				}
			}
			return lastElement;
		}
		public override bool CursorToDisplayPosition(int newPosition, bool forceSelection) {
			ApplyCurrentElementEditor();
			if(!CurrentValue.HasValue)
				return false;
			int toIndex = GetFormatIndexFromPosition(newPosition);
			if(toIndex < 0)
				return false;
			if(forceSelection && toIndex > SelectedFormatInfoIndex)
				return false;
			fSelectedFormatInfoIndex = toIndex;
			return true;
		}
		public override bool CursorLeft(bool forceSelection, bool isNeededKeyCheck) {
			bool result = false;
			if(IsElementEdited) {
				if(!isNeededKeyCheck) {
					result = true;
					ApplyCurrentElementEditor();
				}
			} else if(!CurrentValue.HasValue) {
				return false;
			}
			for(int i = SelectedFormatInfoIndex - 1; i >= 0; --i) {
				if(FormatInfo[i].Editable) {
					if(!isNeededKeyCheck) {
						fSelectedFormatInfoIndex = i;
					}
					return true;
				}
			}
			return result;
		}
		public override bool CursorRight(bool forceSelection, bool isNeededKeyCheck) {
			bool result = false;
			if(IsElementEdited) {
				if(!isNeededKeyCheck) {
					result = true;
					ApplyCurrentElementEditor();
				}
			} else if(!CurrentValue.HasValue) {
				return false;
			}
			for(int i = SelectedFormatInfoIndex + 1; i < FormatInfo.Count; ++i) {
				if(FormatInfo[i].Editable) {
					if(!isNeededKeyCheck) {
						fSelectedFormatInfoIndex = i;
					}
					return true;
				}
			}
			return result;
		}
		public override bool CursorHome(bool forceSelection) {
			ApplyCurrentElementEditor();
			fSelectedFormatInfoIndex = -1;
			for(int i = 0; i < FormatInfo.Count; ++i) {
				if(FormatInfo[i].Editable) {
					fSelectedFormatInfoIndex = i;
					break;
				}
			}
			return true;
		}
		public override bool CursorEnd(bool forceSelection) {
			ApplyCurrentElementEditor();
			if(!CurrentValue.HasValue)
				return CursorHome(forceSelection);
			fSelectedFormatInfoIndex = -1;
			for(int i = FormatInfo.Count - 1; i >= 0; --i) {
				if(FormatInfo[i].Editable) {
					fSelectedFormatInfoIndex = i;
					break;
				}
			}
			return true;
		}
		public override bool SpinUp() {
			DateTimeElementEditor currentEditor = GetCurrentElementEditor();
			if(currentEditor == null)
				return false;
			else
				return currentEditor.SpinUp();
		}
		public override bool SpinDown() {
			DateTimeElementEditor currentEditor = GetCurrentElementEditor();
			if(currentEditor == null)
				return false;
			else
				return currentEditor.SpinDown();
		}
		public override bool FlushPendingEditActions() {
			return ApplyCurrentElementEditor();
		}
		public override object GetCurrentEditValue() {
			return CurrentValue;
		}
		public void SetInitialEditValue(DateTime? initialEditValue) {
			KillCurrentElementEditor();
			fCurrentValue = initialEditValue;
			fUndoValue = initialEditValue;
			fInitialEditValue = initialEditValue;
			CursorHome(false);
		}
		public override void SetInitialEditValue(object initialEditValue) {
			if(initialEditValue is DateTime || initialEditValue == null) {
				SetInitialEditValue((DateTime?)initialEditValue);
			} else if(initialEditValue is TimeSpan) {
				SetInitialEditValue(new DateTime(((TimeSpan)initialEditValue).Ticks));
			} else {
				SetInitialEditText(string.Format(CultureInfo.InvariantCulture, "{0}", initialEditValue));
			}
		}
		[Obsolete("Use DateTimeMaskManager(string mask, bool isOperatorMask, CultureInfo culture, bool allowNull) instead")]
		public DateTimeMaskManagerCore(string mask, bool isOperatorMask, CultureInfo culture) : this(mask, isOperatorMask, culture, true) { }
		public DateTimeMaskManagerCore(string mask, bool isOperatorMask, CultureInfo culture, bool allowNull)
			: base() {
			this.AllowNull = allowNull;
			this.IsOperatorMask = isOperatorMask;
			this.fInitialMask = mask;
			this.fInitialDateTimeFormatInfo = DateTimeMaskManager.GetGoodCalendarDateTimeFormatInfo(culture);
			fFormatInfo = new DateTimeMaskFormatInfo(mask, this.fInitialDateTimeFormatInfo);
			CursorHome(false);
		}
		public override void SelectAll() {
			CursorHome(false);
		}
		public void ClearFromSelectAll() {
			KillCurrentElementEditor();
			DateTime? newValue = null;
			if(!AllowNull)
				newValue = GetClearValue();
			if(newValue != CurrentValue) {
				if(RaiseEditTextChanging(newValue)) {
					this.fUndoValue = CurrentValue;
					this.fCurrentValue = newValue;
					RaiseEditTextChanged();
				}
			}
			CursorHome(false);
		}
		public static bool AlwaysTodayOnClearSelectAll = false;
		protected virtual DateTime GetClearValue() {
			if(AlwaysTodayOnClearSelectAll || !fInitialEditValue.HasValue)
				return DateTime.Today;
			switch(FormatInfo.DateTimeParts) {
				case DateTimePart.Time:
					return fInitialEditValue.Value.Date;
				case DateTimePart.Date:
					return DateTime.Today.AddTicks(fInitialEditValue.Value.TimeOfDay.Ticks);
				default:
					return DateTime.Today;
			}
		}
	}
	public static class DateTimeMaskManagerHelper {
		public static DateTimeMaskFormatInfo GetFormatInfo(DateTimeMaskManagerCore manager) {
			return manager.FormatInfo;
		}
		public static DateTimeMaskFormatInfo GetFormatInfo(DateTimeMaskManager manager) {
			return GetFormatInfo(manager.Nested);
		}
	}
	public class DateTimeMaskManager: MaskManagerSelectAllEnhancer<DateTimeMaskManagerCore> {
		public DateTimeMaskManager(string mask, bool isOperatorMask, CultureInfo culture, bool allowNull)
			: this(new DateTimeMaskManagerCore(mask, isOperatorMask, culture, allowNull)) {
		}
		public DateTimeMaskManager(DateTimeMaskManagerCore coreManager) : base(coreManager) { }
		protected override bool IsNestedCanSelectAll { get { return false; } }
		public static bool DoNotClearValueOnInsertAfterSelectAll = false;   
		protected override bool MakeChange(Func<bool> changeWithTrueWhenSuccessfull) {
			if(IsSelectAllEnforced) {
				ClearSelectAllFlag();
				if(!DoNotClearValueOnInsertAfterSelectAll) {
					Nested.ClearFromSelectAll();
				}
				base.MakeChange(changeWithTrueWhenSuccessfull);
				return true;
			} else {
				return base.MakeChange(changeWithTrueWhenSuccessfull);
			}
		}
		protected override bool MakeCursorOp(Func<bool> cursorOpWithTrueWhenSuccessfull) {
			if(IsSelectAllEnforced) {
				ClearSelectAllFlag();
				base.MakeCursorOp(cursorOpWithTrueWhenSuccessfull);
				return true;
			} else {
				return base.MakeCursorOp(cursorOpWithTrueWhenSuccessfull);
			}
		}
		static bool IsGoodCalendar(System.Globalization.Calendar calendar) {
			if(calendar is GregorianCalendar)
				return true;
			if(calendar is KoreanCalendar)
				return true;
			if(calendar is TaiwanCalendar)
				return true;
			if(calendar is ThaiBuddhistCalendar)
				return true;
			return false;
		}
		public static DateTimeFormatInfo GetGoodCalendarDateTimeFormatInfo(CultureInfo inputCulture) {
			if(IsGoodCalendar(inputCulture.DateTimeFormat.Calendar))
				return inputCulture.DateTimeFormat;
			DateTimeFormatInfo result = (DateTimeFormatInfo)inputCulture.DateTimeFormat.Clone();
			foreach(System.Globalization.Calendar candidateCalendar in inputCulture.OptionalCalendars) {
				if(IsGoodCalendar(candidateCalendar)) {
					result.Calendar = candidateCalendar;
					return result;
				}
			}
			return DateTimeFormatInfo.InvariantInfo;
		}
		public override bool Backspace() {
			if(IsSelectAllEnforced) {
				ClearSelectAllFlag();
				Nested.ClearFromSelectAll();
				return true;
			} else {
				return base.Backspace();
			}
		}
		public override bool Delete() {
			if(IsSelectAllEnforced) {
				ClearSelectAllFlag();
				Nested.ClearFromSelectAll();
				return true;
			} else {
				return base.Delete();
			}
		}
	}
}
