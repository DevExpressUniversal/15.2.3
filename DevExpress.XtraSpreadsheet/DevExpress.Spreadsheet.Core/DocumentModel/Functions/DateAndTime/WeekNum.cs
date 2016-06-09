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
	#region FunctionWeekNum
	public class FunctionWeekNum : FunctionDateBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "WEEKNUM"; } }
		public override int Code { get { return 0x01D1; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue dateSerialValue = GetNumericValue(arguments[0], context);
			if (dateSerialValue.IsError)
				return dateSerialValue;
			if (dateSerialValue.NumericValue < 0)
				return VariantValue.ErrorNumber;
			VariantValue returnType = GetReturnType(arguments, context);
			if (returnType.IsError)
				return returnType;
			return WeekNum(context, dateSerialValue.NumericValue, returnType.NumericValue != 21, (int)returnType.NumericValue);
		}
		protected override VariantValue GetNumericValue(VariantValue value, WorkbookDataContext context) {
			if (value.IsBoolean)
				return VariantValue.ErrorInvalidValueInFunction;
			if (value.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			if (value.IsArray) {
				VariantValue resultValue = value.ArrayValue.GetValue(0, 0);
				VariantValue result = resultValue.ToNumeric(context);
				return !resultValue.IsBoolean ? result : VariantValue.ErrorInvalidValueInFunction;
			}
			if (value.IsCellRange) {
				if (value.CellRangeValue.CellCount != 1)
					return VariantValue.ErrorInvalidValueInFunction;
				VariantValue cellValue = value.CellRangeValue.GetFirstCellValue();
				if (cellValue == VariantValue.ErrorGettingData)
					return cellValue;
				if (cellValue.IsBoolean)
					return VariantValue.ErrorInvalidValueInFunction;
			}
			return base.GetNumericValue(value, context);
		}
		VariantValue WeekNum(WorkbookDataContext context, double dateSerialValue, bool isSystem1, int returnType) {			
			int firstDayOfWeek = GetFirstDayOfWeek(context, dateSerialValue, returnType);
			if (context.DateSystem == DateSystem.Date1900 && dateSerialValue == 0)
				return WeekNumForZeroSerial(returnType);
			VariantValue weekNum = WeekNumCore(context, dateSerialValue, isSystem1, firstDayOfWeek);
			if (weekNum == 0 && !isSystem1)
				weekNum = GetLastWeekNumInPreviousYearForSystem2(context, dateSerialValue); 
			return weekNum;
		}
		VariantValue WeekNumForZeroSerial(int returnType) {
			switch (returnType) {
				case 1:
				case 17:
					return 0;
				case 21:
					return 52;
				default:
					return 1;
			}
		}
		VariantValue WeekNumCore(WorkbookDataContext context, double dateSerialValue, bool isSystem1, int firstDayOfWeek) {
			DateTime date = context.FromDateTimeSerial(dateSerialValue);
			int lastDayFirstWeek = GetLastDayFirstWeek(new DateTime(date.Year, 1, 1), firstDayOfWeek, isSystem1);
			int weekNumber = (int)Decimal.Ceiling(((decimal)(date.DayOfYear - lastDayFirstWeek)) / 7) + 1;
			if (context.DateSystem == DateSystem.Date1900 && isSystem1 && firstDayOfWeek == 1 && dateSerialValue > GetLastDayOfWeekContains60(context, firstDayOfWeek))
				weekNumber++;
			return weekNumber;
		}
		VariantValue GetLastWeekNumInPreviousYearForSystem2(WorkbookDataContext context, double dateSerialValue) {
			int curentYear = context.FromDateTimeSerial(dateSerialValue).Year;
			if (context.DateSystem == DateSystem.Date1904 && curentYear == 1904)
				return 53;
			VariantValue lastDayInPreviousYear = context.ToDateTimeSerial(new DateTime(curentYear - 1, 12, 31));
			return WeekNumCore(context, lastDayInPreviousYear.NumericValue, false, 1);
		}
		int GetLastDayFirstWeek(DateTime firstDayOfYear, int firstDayOfWeek, bool isSystem1) {
			int result = 1;
			DayOfWeek dayOfWeek = firstDayOfYear.DayOfWeek;
			int lastDayOfWeek = firstDayOfWeek - 1 >= 0 ? firstDayOfWeek - 1 : 6;
			if(!isSystem1)
				while ((int)dayOfWeek != 4) {
					dayOfWeek = (DayOfWeek)(((int)dayOfWeek + 1) % 7);
					result++;
				}
			while ((int)dayOfWeek != lastDayOfWeek) {
				dayOfWeek = (DayOfWeek)(((int)dayOfWeek + 1) % 7);
				result++;
			}
			return result;
		}
		int GetFirstDayOfWeek(WorkbookDataContext context, double dateSerialValue, int returnType) {		   
			int result = GetFirstDayOfWeekCore(returnType);
			if (context.DateSystem == DateSystem.Date1904)
				return result;
			int lastDayOfWeekContains60 = GetLastDayOfWeekContains60(context, result);
			return (dateSerialValue <= lastDayOfWeekContains60) ? (result + 1) % 7 : result;
		}
		VariantValue GetReturnType(IList<VariantValue> arguments, WorkbookDataContext context) {
			if (arguments.Count == 1)
				return 1;
			VariantValue returnTypeValue = GetNumericValue(arguments[1], context).ToNumeric(context);
			if (returnTypeValue.IsError)
				return returnTypeValue;
			double returnType = returnTypeValue.NumericValue;
			if ((returnType > 17 && returnType != 21) || (returnType < 11 && (returnType < 1 || returnType > 2)))
				return VariantValue.ErrorNumber;
			return returnType;
		}
		int GetFirstDayOfWeekCore(int returnType) {
			switch (returnType) {
				case 1:
				case 17:
					return 0;
				case 2:
				case 11:
				case 21:
					return 1;
				case 12:
					return 2;
				case 13:
					return 3;
				case 14:
					return 4;
				case 15:
					return 5;
				default:
					return 6;
			}
		}
		int GetLastDayOfWeekContains60(WorkbookDataContext context, int firstDayOfWeek) {
			int result = 60;
			DayOfWeek dayOfWeek = context.FromDateTimeSerial(60).DayOfWeek;
			while ((int)dayOfWeek != firstDayOfWeek) {
				dayOfWeek = (DayOfWeek)(((int)dayOfWeek + 1) % 7);
				result++;
			}
			return result;
		}	   
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
