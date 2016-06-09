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
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionWorkdayIntl
	public class FunctionWorkdayIntl : FunctionDateIntlBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "WORKDAY.INTL"; } }
		public override int Code { get { return 0x400E; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue startDateValue = arguments[0];
			if (startDateValue.IsError)
				return startDateValue;
			VariantValue daysCountValue = arguments[1];
			if (daysCountValue.IsError)
				return daysCountValue;
			VariantValue startDate = GetNumericValue(startDateValue, context, true);
			if (startDate.IsError)
				return startDate;
			VariantValue daysCount = GetNumericValue(daysCountValue, context, false);
			if (daysCount.IsError)
				return daysCount;
			return WorkdayIntlCore(arguments, context, (int)Math.Floor(startDate.NumericValue), (int)Math.Floor(daysCount.NumericValue));
		}
		VariantValue GetNumericValue(VariantValue value, WorkbookDataContext context, bool IsStartDate) {
			VariantValue numericValue;
			if (value.IsBoolean)
				return VariantValue.ErrorInvalidValueInFunction;
			if (value.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			if (value.IsArray) {
				VariantValue resultValue = value.ArrayValue.GetValue(0, 0);
				VariantValue result = resultValue.ToNumeric(context);
				if (IsStartDate && context.DateSystem == DateSystem.Date1904 && resultValue.IsText && !result.IsError)
					return VariantValue.ErrorNumber;
				return !resultValue.IsBoolean ? result : VariantValue.ErrorInvalidValueInFunction;
			}
			numericValue = base.GetNumericValue(value, context);
			if (value.IsText && IsStartDate && context.DateSystem == DateSystem.Date1904 && numericValue.IsNumeric)
				return VariantValue.ErrorNumber;
			return numericValue;
		}
		VariantValue WorkdayIntlCore(IList<VariantValue> arguments, WorkbookDataContext context, int startDate, int daysCount) {
			if(startDate < 0)
				return VariantValue.ErrorNumber;
			int result = startDate;
			int[] weekdays = new int[7];
			VariantValue operationResult = GetWeekdays(arguments, context, weekdays, false);
			if(operationResult.IsError)
				return operationResult;
			List<int> holidays = new List<int>();
			if(arguments.Count == 4) {
				VariantValue holidaysValue = arguments[3];
				VariantValue parseResult = TryAddHolidays(context, holidaysValue, weekdays, holidays);
				if(parseResult.IsError)
					return parseResult;
			}
			if(daysCount < 0)
				result = WorkdayIntlBeforeStartData(daysCount, result, weekdays, holidays, context);
			else
				result = WorkdayIntlAfterStartData(daysCount, result, weekdays, holidays, context);
			return result >= 0 ? result : VariantValue.ErrorNumber;
		}
		int WorkdayIntlAfterStartData(int daysCount, int result, int[] weekdays, List<int> holidays, WorkbookDataContext context) {
			while (daysCount > 0) {
				result++;
				daysCount--;
				result = ConsiderHolidaysAndWeekendAfterStartData(weekdays, result, context);
				if (holidays.Contains(result)) {
					while (holidays.Contains(result)) {
						result++;
						result = ConsiderHolidaysAndWeekendAfterStartData(weekdays, result, context);
					}
				}
			}
			return result;
		}
		int ConsiderHolidaysAndWeekendAfterStartData(int[] weekdays, int result, WorkbookDataContext context) {
			DayOfWeek weekDay = context.FromDateTimeSerialForDayOfWeek(result).DayOfWeek;
			int weekDayNumber = weekDay == DayOfWeek.Sunday ? 6 : (int)weekDay - 1;
			while (weekdays[weekDayNumber] == 1) {
				result++;
				weekDayNumber = (weekDayNumber + 1) % 7;
			}
			return result;
		}
		int WorkdayIntlBeforeStartData(int daysCount, int result, int[] weekdays, List<int> holidays, WorkbookDataContext context) {
			while (daysCount < 0) {
				result--;
				daysCount++;
				result = ConsiderHolidaysAndWeekendBeforeStartData(result, weekdays, holidays, context);
				if (holidays.Contains(result)) {
					while (holidays.Contains(result)) {
						result--;
						result = ConsiderHolidaysAndWeekendBeforeStartData(result, weekdays, holidays, context);
					}
				}
			}
			return result;
		}
		int ConsiderHolidaysAndWeekendBeforeStartData(int result, int[] weekdays, List<int> holidays, WorkbookDataContext context) {
			DayOfWeek weekDay = context.FromDateTimeSerialForDayOfWeek(result).DayOfWeek;
			int weekDayNumber = weekDay == DayOfWeek.Sunday ? 6 : (int)weekDay - 1;
			while (weekdays[weekDayNumber] == 1) {
				result--;
				weekDayNumber = (weekDayNumber - 1 >= 0 ? weekDayNumber - 1 : 6) % 7;
			}
			return result;
		}
		VariantValue TryAddHolidays(WorkbookDataContext context, VariantValue holidaysValue, int[] weekdays, List<int> holidays) {
			if (holidaysValue.IsCellRange) {
				VariantValue parseResult = TryAddHolidaysFromCellRange(weekdays, context, holidaysValue, holidays);
				if (parseResult.IsError)
					return parseResult;
			}
			else if (holidaysValue.IsArray) {
				VariantValue parseResult = TryAddHolidaysFromArray(weekdays, context, holidaysValue, holidays);
				if (parseResult.IsError)
					return parseResult;
			}
			else if (holidaysValue.IsBoolean) {
				return VariantValue.ErrorInvalidValueInFunction;
			}
			else {
				VariantValue number = holidaysValue.ToNumeric(context);
				if (number.IsError)
					return number;
				if (number.NumericValue < 0)
					return VariantValue.ErrorNumber;
				if (IsAdditionalHolidays(weekdays, holidays, (int)number.NumericValue, context))
					holidays.Add((int)number.NumericValue);
			}
			return VariantValue.Empty;
		}
		VariantValue TryAddHolidaysFromArray(int[] weekdays, WorkbookDataContext context, VariantValue holidaysValue, List<int> holidays) {
			IVariantArray array = holidaysValue.ArrayValue;
			for (int column = 0; column < array.Width; column++) {
				for (int row = 0; row < array.Height; row++) {
					VariantValue numberValue = array.GetValue(row, column);
					VariantValue number = GetNumericValue(numberValue, context);
					if (number.IsError)
						return number;
					if (numberValue.IsBoolean)
						return VariantValue.ErrorInvalidValueInFunction;
					if (number.NumericValue < 0)
						return VariantValue.ErrorNumber;
					if (context.DateSystem == DateSystem.Date1904 && numberValue.IsText)
						continue;
					if (IsAdditionalHolidays(weekdays, holidays, (int)number.NumericValue, context))
						holidays.Add((int)number.NumericValue);
				}
			}
			return VariantValue.Empty;
		}
		VariantValue TryAddHolidaysFromCellRange(int[] weekdays, WorkbookDataContext context, VariantValue holidaysValue, List<int> holidays) {
			CellRangeBase cellRange = holidaysValue.CellRangeValue;
			if (cellRange.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorInvalidValueInFunction;
			IEnumerator<VariantValue> cellValues = cellRange.GetExistingValuesEnumerator();
			while (cellValues.MoveNext()) {
				VariantValue cellValue = cellValues.Current;
				if (cellValue.IsError)
					return cellValue;
				if (cellValue.IsBoolean)
					return VariantValue.ErrorInvalidValueInFunction;
				VariantValue numericValue = GetNumericValue(cellValue, context);
				if (numericValue.IsError)
					return numericValue;
				if (numericValue.IsBoolean)
					return VariantValue.ErrorInvalidValueInFunction;
				if (numericValue.NumericValue < 0)
					return VariantValue.ErrorNumber;
				if (IsAdditionalHolidays(weekdays, holidays, (int)numericValue.NumericValue, context))
					holidays.Add((int)numericValue.NumericValue);
			}
			return VariantValue.Empty;
		}
		bool IsAdditionalHolidays(int[] weekdays, List<int> holidays, int number, WorkbookDataContext context) {
			return !holidays.Contains(number) && IsWorkdayCore(weekdays, number, context) && number >= 0;
		}		
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Array | OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
