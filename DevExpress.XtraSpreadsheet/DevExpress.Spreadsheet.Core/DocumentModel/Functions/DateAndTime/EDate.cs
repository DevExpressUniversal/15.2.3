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
	#region FunctionEDate
	public class FunctionEDate : FunctionSerialNumberBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		protected double MaxCountMonths { get { return 97200; } }
		public override string Name { get { return "EDATE"; } }
		public override int Code { get { return 0x01C1; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue startDate = arguments[0];
			VariantValue startDateSerialValue = ToSerialNumber(startDate, context);
			if (startDateSerialValue.IsError)
				return startDateSerialValue;
			VariantValue monthsCountValue = GetCountMonths(arguments[1], context);
			if (monthsCountValue.IsError)
				return monthsCountValue;
			return GetSerialNumberResult((int)startDateSerialValue.NumericValue, (int)monthsCountValue.NumericValue, context, startDate.IsInlineText);
		}
		VariantValue GetCountMonths(VariantValue value, WorkbookDataContext context) {
			if (value.IsError)
				return value;
			value = ToSerialNumberCore(value, context);
			if (value.IsError)
				return value;
			if (Math.Abs(value.NumericValue) >= MaxCountMonths)
				return VariantValue.ErrorNumber;
			return value;
		}
		protected virtual VariantValue GetSerialNumberResult(int startDateSerialNumber, int countMonths, WorkbookDataContext context, bool dateIsInlineText) {
			VariantValue countDays = GetCountDays(startDateSerialNumber, countMonths, context);
			if (countDays.IsError)
				return countDays;
			int endDateSerialNumber = startDateSerialNumber + (int)countDays.NumericValue;
			if (context.DateSystem == DateSystem.Date1900 && startDateSerialNumber == NumberDay29Feb1900 && endDateSerialNumber <= NumberDay29Feb1900)
				endDateSerialNumber -= countMonths < 0 ? 0 : 1;
			if (WorkbookDataContext.IsErrorDateTimeSerial(endDateSerialNumber, context.DateSystem))
				return VariantValue.ErrorNumber;
			if (context.DateSystem == DateSystem.Date1904 && dateIsInlineText)
				return endDateSerialNumber - 1462;
			else
				return endDateSerialNumber;
		}
		VariantValue GetCountDays(int startDateSerialNumber, int countMonths, WorkbookDataContext context) {
			VariantValue startDateValue = GetStartDate(startDateSerialNumber, context);
			VariantValue endDateValue = GetEndDate(startDateValue, countMonths, context);
			if (startDateValue.IsError || endDateValue.IsError)
				return VariantValue.ErrorNumber;
			int result = (int)(endDateValue.NumericValue - startDateValue.NumericValue); 
			return result; 
		}
		VariantValue GetStartDate(int startDateSerialNumber, WorkbookDataContext context) {
			DateTime result = (context.DateSystem == DateSystem.Date1900 ? SystemData : SystemData1904);
			if (startDateSerialNumber == 0)
				return context.FromDateTime(result);
			if (context.DateSystem == DateSystem.Date1900) {
				result = result.AddDays(startDateSerialNumber - 2);
				if (startDateSerialNumber < NumberDay29Feb1900)
					result = result.AddDays(1);
			}
			else
				result = result.AddDays(startDateSerialNumber);
			return context.FromDateTime(result);
		}
		VariantValue GetEndDate(VariantValue startDateValue, int countMonths, WorkbookDataContext context) {
			DateTime dateTime = startDateValue.ToDateTime(context);
			if (dateTime.Month + countMonths > MaxCountMonths)
				return VariantValue.ErrorNumber;
			DateTime endDate;
			try {
				endDate = dateTime.AddMonths(countMonths);
			}catch(Exception){
				return VariantValue.ErrorInvalidValueInFunction;
			}
			return context.FromDateTime(endDate);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			return collection;
		}
	}
	#endregion
}
