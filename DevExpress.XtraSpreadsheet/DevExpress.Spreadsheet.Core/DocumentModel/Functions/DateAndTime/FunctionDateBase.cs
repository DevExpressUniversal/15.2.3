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
using DevExpress.XtraSpreadsheet.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionDateBase (abstract class)
	public abstract class FunctionDateBase : WorksheetFunctionBase {
		public DateTime SystemData { get { return new DateTime(1900, 1, 1); } }
		public DateTime SystemData1904 { get { return new DateTime(1904, 1, 1); } }
	}
	#endregion
	#region FunctionDateIntlBase (abstract class)
	public abstract class FunctionDateIntlBase : FunctionSerialNumberBase {
		protected bool IsWorkdayCore(int[] weekdays, int number, WorkbookDataContext context) {
			DayOfWeek weekDay = context.FromDateTimeSerialForDayOfWeek(number).DayOfWeek;
			int weekDayNumber = weekDay == DayOfWeek.Sunday ? 6 : (int)weekDay - 1;
			return weekdays[weekDayNumber] == 0;
		}
		protected internal VariantValue GetWeekdays(IList<VariantValue> arguments, WorkbookDataContext context, int[] weekdays, bool isAllDaysWeekendIgnored) {
			if(arguments.Count == 2) {
				GetWeekdaysCore(weekdays, 0, 4, 0);
				GetWeekdaysCore(weekdays, 5, 6, 1);
				return VariantValue.Empty;
			}
			VariantValue value = arguments[2];
			if (value.IsText) {
				VariantValue result = GetWeekdaysCore(weekdays, value.GetTextValue(context.Workbook.SharedStringTable), isAllDaysWeekendIgnored);
				if (result.IsError)
					return result;
			}
			else if (value == VariantValue.ErrorGettingData)
				return value;
			else if (value.IsError)
				return VariantValue.ErrorNumber;
			else {
				VariantValue result = GetWeekdaysCore(context, value, weekdays);
				if (result.IsError)
					return result;
			}
			return VariantValue.Empty;
		}
		VariantValue GetWeekdaysCore(WorkbookDataContext context, VariantValue value, int[] weekdays) {
			VariantValue weekendsValue = GetNumericValue(value, context);
			if(weekendsValue.IsError)
				return weekendsValue;
			int weekendsNumber = (int)weekendsValue.NumericValue;
			if(weekendsNumber > 0 && weekendsNumber < 8) {
				int firstWeekend = (weekendsNumber + 4) % 7;
				GetWeekdaysCore(weekdays, (firstWeekend + 2) % 7, (firstWeekend + 6) % 7, 0);
				GetWeekdaysCore(weekdays, firstWeekend, (firstWeekend + 1) % 7, 1);
			}
			else if(weekendsNumber > 10 && weekendsNumber < 18) {
				int firstWeekend = (weekendsNumber + 2) % 7;
				GetWeekdaysCore(weekdays, (firstWeekend + 1) % 7, (firstWeekend + 6) % 7, 0);
				GetWeekdaysCore(weekdays, firstWeekend, firstWeekend, 1);
			}
			else
				return VariantValue.ErrorNumber;
			return VariantValue.Empty;
		}
		void GetWeekdaysCore(int[] weekdays, int start, int end, int dayStatus) {
			if(start <= end)
				FullWeekdaysCoreForwardDirection(weekdays, start, end, dayStatus);
			else
				FullWeekdaysCore(weekdays, start, end, dayStatus);
		}
		void FullWeekdaysCore(int[] weekdays, int start, int end, int dayStatus) {
			for(int i = start; i < 7; i++)
				weekdays[i] = dayStatus;
			for(int i = 0; i <= end; i++)
				weekdays[i] = dayStatus;
		}
		void FullWeekdaysCoreForwardDirection(int[] weekdays, int start, int end, int dayStatus) {
			for(int i = start; i <= end; i++)
				weekdays[i] = dayStatus;
		}
		VariantValue GetWeekdaysCore(int[] weekdays, string text, bool isAllDaysWeekendIgnored) {
			bool isAllDaysWeekEnd = true;
			if(text.Length != 7)
				return VariantValue.ErrorInvalidValueInFunction;
			for(int i = 0; i < text.Length; i++) {
				int day;
				if(Int32.TryParse(text[i].ToString(), out day)) {
					if(day == 0)
						isAllDaysWeekEnd = false;
					if(day == 0 || day == 1)
						weekdays[i] = day;
					else
						return VariantValue.ErrorInvalidValueInFunction;
				}
				else
					return VariantValue.ErrorInvalidValueInFunction;
			}
			if(isAllDaysWeekEnd && !isAllDaysWeekendIgnored)
				for(int i = 0; i < text.Length; i++)
					weekdays[i] = 0;
			return VariantValue.Empty;
		}
	}
	#endregion
	#region FunctionSerialNumberBase (abstract class)
	public abstract class FunctionSerialNumberBase : FunctionDateBase {
		protected double NumberDay29Feb1900 { get { return 60; } }
		protected const double halfOfTheSecondSerialNumber = 0.000005780253559350970;
		protected VariantValue ToSerialNumber(VariantValue value, WorkbookDataContext context) {
			value = ToSerialNumberCore(value, context);
			if (value.IsError)
				return value;
			double numericCorrected = value.NumericValue;
			if (numericCorrected > 0)
				numericCorrected = numericCorrected + halfOfTheSecondSerialNumber;
			if (WorkbookDataContext.IsErrorDateTimeSerial(numericCorrected, context.DateSystem))
				return VariantValue.ErrorNumber;
			return (int)value.NumericValue;
		}
		protected virtual VariantValue ToSerialNumberCore(VariantValue value, WorkbookDataContext context) {
			if (value.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			if (value.IsBoolean)
				return VariantValue.ErrorInvalidValueInFunction;
			if (value.IsCellRange) {
				value = value.CellRangeValue.GetFirstCellValue();
				if (value.IsEmpty)
					return 0;
				return ToSerialNumber(value, context);
			}
			return value.ToNumeric(context);
		}
		protected VariantValue DateToSerialNumber(VariantValue value, WorkbookDataContext context) {
			if (value.IsCellRange && value.CellRangeValue.CellCount > 1)
				return VariantValue.ErrorInvalidValueInFunction;
			return ToSerialNumber(value, context);
		}
		protected VariantValue GetBasis(VariantValue basis, WorkbookDataContext context) {
			basis = context.ToNumericWithoutCrossingCore(basis);
			if (basis.IsError)
				return basis;
			int basisId = (int)basis.NumericValue;
			if (basisId < 0 || basisId > 4)
				return VariantValue.ErrorNumber;
			return basisId;
		}
	}
	#endregion
	#region FunctionWorkdayBase (abstract class)
	public abstract class FunctionWorkdayBase : FunctionDateIntlBase {
		protected bool IsWorkday(int serialNumber, WorkbookDataContext context, int[] weekdays) {
			if (context.DateSystem == DateSystem.Date1900 && serialNumber == NumberDay29Feb1900)
				return true;
			return IsWorkdayCore(weekdays, serialNumber, context);
		}
		protected bool IsWorkday(int serialNumber, int startSerialNumber, int endSerialNumber, WorkbookDataContext context, int[] weekdays) {
			bool isBetween;
			if (startSerialNumber <= endSerialNumber)
				isBetween = serialNumber >= startSerialNumber && serialNumber <= endSerialNumber;
			else
				isBetween = serialNumber >= endSerialNumber && serialNumber <= startSerialNumber;
			return IsWorkday(serialNumber, context, weekdays) && isBetween;
		}
		protected int GetCountWorkdays(int startSerialNumber, int endSerialNumber, WorkbookDataContext context, int[] weekdays) {
			int countDays = endSerialNumber - startSerialNumber;
			if(countDays == 0)
				return IsWorkday(startSerialNumber, startSerialNumber, endSerialNumber, context, weekdays) ? 1 : 0;
			int absDaysCount = Math.Abs(countDays) + 1;
			int weekCount = absDaysCount / 7;
			int daysCountLastWeek = absDaysCount - weekCount * 7;
			int startNumberLastWeek = (countDays > 0) ? startSerialNumber : endSerialNumber;
			startNumberLastWeek += weekCount * 7;
			for(int i = 0; i < daysCountLastWeek; i++)
				if(!IsWorkday(startNumberLastWeek + i, startSerialNumber, endSerialNumber, context, weekdays))
					absDaysCount--;
			int weekendsCount = GetWeekendsCount(weekdays);
			return Math.Sign(countDays) * (absDaysCount - weekendsCount * weekCount);
		}
		int GetWeekendsCount(int[] weekdays) {
			int weekendsCount = 0;
			for(int i = 0; i < weekdays.Length; i++)
				if(weekdays[i] == 1)
					weekendsCount++;
			return weekendsCount;
		}
		protected VariantValue TryAddHolidays(VariantValue value, WorkbookDataContext context, int startSerialNumber, int endSerialNumber, List<double> holidays, int[] weekdays, bool operandDataTypeIsValue) {
			VariantValue errorHolidays = new VariantValue();
			if (value.IsError)
				return value;
			if (value.IsBoolean)
				return VariantValue.ErrorInvalidValueInFunction;
			if (value.IsCellRange)
				errorHolidays = AddHolidaysFromCellRange(value.CellRangeValue, context, startSerialNumber, endSerialNumber, holidays, weekdays, operandDataTypeIsValue);
			if (value.IsArray)
				errorHolidays = AddHolidaysFromArray(value.ArrayValue, context, startSerialNumber, endSerialNumber, holidays, weekdays, operandDataTypeIsValue);
			if (value.IsNumeric || value.IsInlineText || value.IsSharedString)
				errorHolidays = AddHolidaysFromValue(value, context, startSerialNumber, endSerialNumber, holidays, weekdays, operandDataTypeIsValue);
			if (errorHolidays.IsError)
				return errorHolidays;
			return VariantValue.Empty;
		}
		VariantValue AddHolidaysFromCellRange(CellRangeBase range, WorkbookDataContext context, int startSerialNumber, int endSerialNumber, List<double> holidays, int[] weekdays, bool operandDataTypeIsValue) {
			if(range.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorInvalidValueInFunction;
			if (range.CellCount != 0) {
				IEnumerator<VariantValue> cellValues = range.GetExistingValuesEnumerator();
				while (cellValues.MoveNext()) {
					VariantValue cellValue = cellValues.Current;
					if (!cellValue.IsEmpty) {
						VariantValue errorHolidays = AddHolidaysFromValue(cellValue, context, startSerialNumber, endSerialNumber, holidays, weekdays, operandDataTypeIsValue);
						if (errorHolidays.IsError)
							return errorHolidays;
					}
				}
			}
			return VariantValue.Empty;
		}
		VariantValue AddHolidaysFromArray(IVariantArray array, WorkbookDataContext context, int startSerialNumber, int endSerialNumber, List<double> holidays, int[] weekdays, bool operandDataTypeIsValue) {
			for(int i = 0; i < array.Count; i++) {
				VariantValue errorHolidays = AddHolidaysFromValue(array[i], context, startSerialNumber, endSerialNumber, holidays, weekdays, operandDataTypeIsValue);
				if (errorHolidays.IsError)
					return errorHolidays;
			}
			return VariantValue.Empty;
		}
		VariantValue AddHolidaysFromValue(VariantValue value, WorkbookDataContext context, int startSerialNumber, int endSerialNumber, List<double> holidays, int[] weekdays, bool operandDataTypeIsValue) {
			VariantValue result = ToSerialNumber(value, context);
			if(result.IsError)
				return result;
			int serialNumber = (int)result.NumericValue;
			if((!IsSpesialCase(serialNumber, weekdays, operandDataTypeIsValue, context) && IsWorkday(serialNumber, startSerialNumber, endSerialNumber, context, weekdays)) && !holidays.Contains(serialNumber))
				holidays.Add(serialNumber);
			return VariantValue.Empty;
		}
		bool IsSpesialCase(VariantValue serialNumber, int[] weekdays, bool operandDataTypeIsValue, WorkbookDataContext context) {
			int day = (int)serialNumber.NumericValue;
			DayOfWeek weekDay = context.FromDateTimeSerialForDayOfWeek(day).DayOfWeek;
			bool weekDayIsSaturdayAndWorkday = (weekDay == DayOfWeek.Saturday && weekdays[5] == 0);
			bool weekDayIsSundayAndWorkday = (weekDay == DayOfWeek.Sunday && weekdays[6] == 0);
			if((weekDayIsSaturdayAndWorkday || weekDayIsSundayAndWorkday) &&
				((context.DateSystem == DateSystem.Date1900 && operandDataTypeIsValue) ||
				(context.DateSystem == DateSystem.Date1904)))
				return true;
			return false;
		}
	}
	#endregion
	#region FunctionDateBase (abstract class)
	public abstract class FunctionDatePartBase : FunctionDateBase {
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0];
			if (value.IsError)
				return value;
			value = value.ToNumeric(context);
			if (value.IsError)
				return value;
			if (WorkbookDataContext.IsErrorDateTimeSerial(value.NumericValue, context.DateSystem))
				return VariantValue.ErrorNumber;
			return GetResult(context, value);
		}
		abstract protected VariantValue GetResult(WorkbookDataContext context, VariantValue value);
		protected DateTime GetResultCore(WorkbookDataContext context, VariantValue value) {
			return context.FromDateTimeSerial(value.NumericValue);
		}
	}
	#endregion
	#region FunctionDateTimeValueBase
	public abstract class FunctionDateTimeValueBase : FunctionDateBase {
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = GetNumericValue(arguments[0], context);
			if (value.IsError)
				return value;
			if (!context.TransitionFormulaEvaluation && 
				WorkbookDataContext.IsErrorDateTimeSerial(value.NumericValue, context.DateSystem))
				return VariantValue.ErrorInvalidValueInFunction;
			if (context.TransitionFormulaEvaluation)
				return value.NumericValue;
			return GetValueCore(value);
		}
		protected override VariantValue GetNumericValue(VariantValue value, WorkbookDataContext context) {
			value = context.DereferenceValue(value, true);
			if (value.IsError)
				return value;
			if (value.IsText) {
				FormattedVariantValue formattedResult = context.TryConvertStringToDateTimeValue(value.GetTextValue(context.StringTable), false);
				VariantValue result = formattedResult.Value;
				if (!result.IsEmpty)
					return result;
			}
			if (context.TransitionFormulaEvaluation) {
				if (value.IsEmpty || value.IsNumeric || value.IsBoolean) {
					value = value.ToNumeric(context);
					return value;
				}
			}
			return VariantValue.ErrorInvalidValueInFunction;
		}
		protected abstract double GetValueCore(VariantValue value);
	}
	#endregion
}
